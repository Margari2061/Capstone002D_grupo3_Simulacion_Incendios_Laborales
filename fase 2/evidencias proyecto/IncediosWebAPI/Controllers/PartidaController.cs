using IncediosWebAPI.Model;
using IncediosWebAPI.Model.DataTransfer;
using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model.IncendioDB.Domain;
using IncediosWebAPI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncediosWebAPI.Controllers;

[JWTAuthorize]
public class PartidaController : Controller
{
    private readonly IncendioContext _context;
    private readonly ILogger<PartidaController> _logger;

    public PartidaController(IncendioContext context, ILogger<PartidaController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ==================== VISTA PRINCIPAL ====================
    public IActionResult Index()
    {
        return View();
    }

    // ==================== CREAR NUEVA PARTIDA ====================
    [HttpPost]
    public async Task<IActionResult> CrearPartida([FromBody] PartidaCreateDTO partidaDto)
    {
        try
        {
            // Validar que el usuario existe
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.Rut == partidaDto.RutUsuario);

            if (!usuarioExiste)
            {
                return NotFound(new { error = $"Usuario con RUT {partidaDto.RutUsuario} no encontrado" });
            }

            // Validar que el nivel existe
            var nivelExiste = await _context.Niveles
                .AnyAsync(n => n.Id == partidaDto.Nivel);

            if (!nivelExiste)
            {
                return NotFound(new { error = $"Nivel con ID {partidaDto.Nivel} no encontrado" });
            }

            // Crear nueva partida con estado INICIADA
            var partida = new Partida
            {
                RutUsuario = partidaDto.RutUsuario,
                IdNivel = partidaDto.Nivel,
                Fecha = DateTime.UtcNow,
                TiempoJugado = TimeSpan.Zero,
                Resultado = ResultadosPartida.EnProgreso,
                FuegosApagados = 0,
                ExtintoresUsados = 0,
                UsoInadecuadoExtintores = 0,
                UsoAlarma = false,
                UsoUniforme = false,
                Heridas = 0,
                Desasosiego = 0
            };

            // Guardar en base de datos
            var entry = _context.Partidas.Add(partida);
            await _context.SaveChangesAsync();

            int partidaId = entry.Entity.Id;

            return Ok(new
            {
                PartidaId = partidaId,
                Mensaje = "Partida iniciada correctamente",
                FechaInicio = partida.Fecha
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear partida");
            return StatusCode(500, new { error = $"Error al comenzar partida: {ex.Message}" });
        }
    }

    // ==================== FINALIZAR PARTIDA ====================
    [HttpPost]
    public async Task<IActionResult> FinalizarPartida([FromBody] PartidaFinalizarDTO finalizarDto)
    {
        try
        {
            // Buscar la partida
            var partida = await _context.Partidas
                .FirstOrDefaultAsync(p => p.Id == finalizarDto.Id);

            if (partida == null)
            {
                return NotFound(new { error = $"Partida con ID {finalizarDto.Id} no encontrada" });
            }

            // Verificar que la partida no esté ya finalizada
            if (partida.Resultado != ResultadosPartida.EnProgreso)
            {
                return BadRequest(new { error = "Esta partida ya fue finalizada anteriormente" });
            }

            // Actualizar con los resultados finales
            partida.TiempoJugado = finalizarDto.TiempoJugado;
            partida.Resultado = finalizarDto.Resultado;
            partida.FuegosApagados = finalizarDto.FuegosApagados;
            partida.ExtintoresUsados = finalizarDto.ExtintoresUsados;
            partida.UsoInadecuadoExtintores = finalizarDto.UsoInadecuadoExtintores;
            partida.UsoAlarma = finalizarDto.UsoAlarma;
            partida.UsoUniforme = finalizarDto.UsoUniforme;
            partida.Heridas = finalizarDto.Heridas;
            partida.Desasosiego = finalizarDto.Desasosiego;

            await _context.SaveChangesAsync();

            // Calcular eficiencia basada en el daño recibido
            string eficiencia = partida.Heridas == 0 ? "PERFECTA" :
                              partida.Heridas <= 25 ? "ALTA" :
                              partida.Heridas <= 50 ? "MEDIA" : "BAJA";

            // Obtener descripción del resultado
            string descripcionResultado = finalizarDto.Resultado switch
            {
                ResultadosPartida.CondicionesCumplidas => "Condiciones Cumplidas",
                ResultadosPartida.EscapeSeguro => "Escape seguro",
                ResultadosPartida.EscapeInmediato => "Escape inmediato",
                ResultadosPartida.EscapeTardio => "Escape tardío",
                ResultadosPartida.Muerte => "Muerte",
                ResultadosPartida.EnProgreso => "En progreso",
                _ => "Desconocido"
            };

            return Ok(new
            {
                message = "Partida finalizada exitosamente",
                resultado = descripcionResultado,
                eficiencia = eficiencia,
                partidaId = partida.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al finalizar partida");
            return StatusCode(500, new { error = $"Error al finalizar partida: {ex.Message}" });
        }
    }

    // ==================== OBTENER PARTIDAS POR USUARIO ====================
    [HttpGet("usuarios/{rut}/partidas")]
    public async Task<IActionResult> GetPartidasPorUsuario(int rut)
    {
        try
        {
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.Rut == rut);

            if (!usuarioExiste)
            {
                return NotFound(new { error = $"Usuario con RUT {rut} no encontrado" });
            }

            var partidas = await _context.Partidas
                .Where(p => p.RutUsuario == rut)
                .OrderByDescending(p => p.Fecha)
                .Select(p => new
                {
                    Id = p.Id,
                    NivelId = p.IdNivel,
                    TiempoJugadoSegundos = (int)p.TiempoJugado.TotalSeconds,
                    Resultado = p.Resultado,
                    FuegosApagados = p.FuegosApagados,
                    ExtintoresUsados = p.ExtintoresUsados,
                    UsoInadecuadoExtintores = p.UsoInadecuadoExtintores,
                    UsoAlarma = p.UsoAlarma,
                    UsoUniforme = p.UsoUniforme,
                    Heridas = p.Heridas,
                    Desasosiego = p.Desasosiego,
                    Fecha = p.Fecha
                })
                .ToListAsync();

            return Ok(new
            {
                totalPartidas = partidas.Count,
                partidas = partidas
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener partidas del usuario {Rut}", rut);
            return StatusCode(500, new { error = $"Error al obtener partidas: {ex.Message}" });
        }
    }

    // ==================== OBTENER ESTADÍSTICAS GENERALES ====================
    [HttpGet("estadisticas")]
    public async Task<IActionResult> GetEstadisticasGenerales()
    {
        try
        {
            var totalPartidas = await _context.Partidas.CountAsync();
            var partidasExitosas = await _context.Partidas
                .CountAsync(p => p.Resultado == ResultadosPartida.CondicionesCumplidas);

            var promedioTiempo = await _context.Partidas
                .AverageAsync(p => p.TiempoJugado.TotalSeconds);

            var promedioFuegosApagados = await _context.Partidas
                .AverageAsync(p => p.FuegosApagados);

            var promedioHeridas = await _context.Partidas
                .AverageAsync(p => p.Heridas);

            var partidasSinDanio = await _context.Partidas
                .CountAsync(p => p.Heridas == 0);

            return Ok(new
            {
                totalPartidas,
                partidasExitosas,
                partidasSinDanio,
                tasaExito = totalPartidas > 0 ? Math.Round((double)partidasExitosas / totalPartidas * 100, 2) : 0,
                tasaSinDanio = totalPartidas > 0 ? Math.Round((double)partidasSinDanio / totalPartidas * 100, 2) : 0,
                promedioTiempoSegundos = Math.Round(promedioTiempo, 2),
                promedioFuegosApagados = Math.Round(promedioFuegosApagados, 2),
                promedioHeridas = Math.Round(promedioHeridas, 2)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas generales");
            return StatusCode(500, new { error = $"Error al obtener estadísticas: {ex.Message}" });
        }
    }

    // ==================== OBTENER DETALLE DE PARTIDA ====================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPartida(int id)
    {
        try
        {
            var partida = await _context.Partidas
                .Include(p => p.Usuario)
                .Include(p => p.Nivel)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (partida == null)
            {
                return NotFound(new { error = $"Partida con ID {id} no encontrada" });
            }

            return Ok(new
            {
                partida.Id,
                partida.RutUsuario,
                Usuario = partida.Usuario?.Nombre,
                Nivel = partida.Nivel?.Nombre,
                partida.TiempoJugado,
                partida.Resultado,
                partida.FuegosApagados,
                partida.ExtintoresUsados,
                partida.UsoInadecuadoExtintores,
                partida.UsoAlarma,
                partida.UsoUniforme,
                partida.Heridas,
                partida.Desasosiego,
                partida.Fecha
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener partida {PartidaId}", id);
            return StatusCode(500, new { error = $"Error al obtener partida: {ex.Message}" });
        }
    }
}