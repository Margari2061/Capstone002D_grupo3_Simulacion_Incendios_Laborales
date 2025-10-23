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

    public PartidaController(IncendioContext context)
    {
        _context = context;
    }

    // ==================== CREAR NUEVA PARTIDA ====================
    [HttpPost, Route("api/runStart")]
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
            return StatusCode(500, new { error = $"Error al comenzar partida: {ex.Message}" });
        }
    }

    // ==================== FINALIZAR PARTIDA ====================
    [HttpPost, Route("api/runEnd")]
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

            DateTime now = DateTime.UtcNow;
            TimeSpan played = now - partida.Fecha;

            // Actualizar con los resultados finales
            partida.TiempoJugado = played;
            partida.Resultado = finalizarDto.Resultado;
            partida.FuegosApagados = finalizarDto.FuegosApagados;
            partida.ExtintoresUsados = finalizarDto.ExtintoresUsados;
            partida.UsoInadecuadoExtintores = finalizarDto.UsoInadecuadoExtintores;
            partida.UsoAlarma = finalizarDto.UsoAlarma;
            partida.UsoUniforme = finalizarDto.UsoUniforme;
            partida.Heridas = finalizarDto.Heridas;
            partida.Desasosiego = finalizarDto.Desasosiego;

            await _context.SaveChangesAsync();

            return Ok("OK");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error al finalizar partida: {ex.Message}" });
        }
    }

    
    // ==================== OBTENER ESTADÍSTICAS GENERALES ====================
    //[HttpGet("estadisticas")]
    //public async Task<IActionResult> GetEstadisticasGenerales()
    //{
    //    try
    //    {
    //        var totalPartidas = await _context.Partidas.CountAsync();
    //        var partidasExitosas = await _context.Partidas
    //            .CountAsync(p => p.Resultado == ResultadosPartida.CondicionesCumplidas);

    //        var promedioTiempo = await _context.Partidas
    //            .AverageAsync(p => p.TiempoJugado.TotalSeconds);

    //        var promedioFuegosApagados = await _context.Partidas
    //            .AverageAsync(p => p.FuegosApagados);

    //        var promedioHeridas = await _context.Partidas
    //            .AverageAsync(p => p.Heridas);

    //        var partidasSinDanio = await _context.Partidas
    //            .CountAsync(p => p.Heridas == 0);

    //        return Ok(new
    //        {
    //            totalPartidas,
    //            partidasExitosas,
    //            partidasSinDanio,
    //            tasaExito = totalPartidas > 0 ? Math.Round((double)partidasExitosas / totalPartidas * 100, 2) : 0,
    //            tasaSinDanio = totalPartidas > 0 ? Math.Round((double)partidasSinDanio / totalPartidas * 100, 2) : 0,
    //            promedioTiempoSegundos = Math.Round(promedioTiempo, 2),
    //            promedioFuegosApagados = Math.Round(promedioFuegosApagados, 2),
    //            promedioHeridas = Math.Round(promedioHeridas, 2)
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { error = $"Error al obtener estadísticas: {ex.Message}" });
    //    }
    //}

    // ==================== OBTENER DETALLE DE PARTIDA ====================
    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetPartida(int id)
    //{
    //    try
    //    {
    //        var partida = await _context.Partidas
    //            .Include(p => p.Usuario)
    //            .Include(p => p.Nivel)
    //            .FirstOrDefaultAsync(p => p.Id == id);

    //        if (partida == null)
    //        {
    //            return NotFound(new { error = $"Partida con ID {id} no encontrada" });
    //        }

    //        return Ok(new
    //        {
    //            partida.Id,
    //            partida.RutUsuario,
    //            Usuario = partida.Usuario?.Nombre,
    //            Nivel = partida.Nivel?.Nombre,
    //            partida.TiempoJugado,
    //            partida.Resultado,
    //            partida.FuegosApagados,
    //            partida.ExtintoresUsados,
    //            partida.UsoInadecuadoExtintores,
    //            partida.UsoAlarma,
    //            partida.UsoUniforme,
    //            partida.Heridas,
    //            partida.Desasosiego,
    //            partida.Fecha
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { error = $"Error al obtener partida: {ex.Message}" });
    //    }
    //}
}