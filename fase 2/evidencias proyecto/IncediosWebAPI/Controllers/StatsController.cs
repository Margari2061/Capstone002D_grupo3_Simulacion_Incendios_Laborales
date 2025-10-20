using IncediosWebAPI.Model;
using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model.IncendioDB.Domain;
using IncediosWebAPI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncediosWebAPI.Controllers;

[WebAuthorize(AppRoles.Admin)]
public class StatsController : Controller
{
    private readonly IncendioContext _context;
    private readonly ILogger<StatsController> _logger;

    public StatsController(IncendioContext context, ILogger<StatsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ==================== VISTA PRINCIPAL ====================
    public IActionResult Index()
    {
        return View();
    }

    // ==================== VISTA DE ESTADÍSTICAS ====================
    [HttpGet]
    public async Task<IActionResult> Estadisticas()
    {
        try
        {
            var estadisticas = await ObtenerEstadisticasETL();
            return View(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar estadísticas");
            TempData["Error"] = "Error al cargar las estadísticas";
            return View(new object());
        }
    }

    // ==================== PROCESAR ETL (POST) ====================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcesarETL()
    {
        try
        {
            // Obtener partidas no procesadas (últimos 30 días)
            var fechaLimite = DateTime.UtcNow.AddDays(-30);
            var partidasNoProcesadas = await _context.Partidas
                .Include(p => p.Usuario)
                .Include(p => p.Nivel)
                .Where(p => p.Fecha >= fechaLimite && p.Resultado != ResultadosPartida.EnProgreso)
                .ToListAsync();

            if (!partidasNoProcesadas.Any())
            {
                TempData["Info"] = "No hay partidas nuevas para procesar";
                return RedirectToAction(nameof(Estadisticas));
            }

            // Procesar cada partida y crear métricas agregadas
            var metricasAgregadas = new List<MetricasEvento>();

            foreach (var partida in partidasNoProcesadas)
            {
                var metricaEvento = new MetricasEvento
                {
                    Fecha = partida.Fecha.Date,
                    ProtocolosSeguidos = partida.Resultado == ResultadosPartida.Exitoso && partida.UsoUniforme,
                    CotizacionEstragos = CalcularContencionEstragos(partida),
                    Lesionados = partida.Heridas > 50 ? 1 : 0
                };

                metricasAgregadas.Add(metricaEvento);
            }

            // Guardar métricas en el Data Warehouse
            _context.MetricasEventos.AddRange(metricasAgregadas);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"ETL procesado: {partidasNoProcesadas.Count} partidas, {metricasAgregadas.Count} métricas generadas";

            return RedirectToAction(nameof(Estadisticas));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en proceso ETL");
            TempData["Error"] = $"Error en proceso ETL: {ex.Message}";
            return RedirectToAction(nameof(Estadisticas));
        }
    }

    // ==================== LIMPIAR DATOS ANTIGUOS ====================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LimpiarDatosAntiguos()
    {
        try
        {
            var fechaLimite = DateTime.UtcNow.AddYears(-1);
            var partidasAntiguas = await _context.Partidas
                .Where(p => p.Fecha < fechaLimite)
                .ToListAsync();

            var cantidadEliminadas = partidasAntiguas.Count;

            if (partidasAntiguas.Any())
            {
                _context.Partidas.RemoveRange(partidasAntiguas);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"Limpieza completada: {cantidadEliminadas} partidas eliminadas";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en limpieza de datos");
            TempData["Error"] = $"Error en limpieza de datos: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // ==================== API PARA DATOS ESTADÍSTICOS (AJAX) ====================
    [HttpGet]
    public async Task<IActionResult> ObtenerDatosEstadisticos()
    {
        try
        {
            var estadisticas = await ObtenerEstadisticasETL();
            return Ok(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos estadísticos");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ==================== MÉTODOS PRIVADOS ====================

    private async Task<object> ObtenerEstadisticasETL()
    {
        var fechaLimite = DateTime.UtcNow.AddDays(-30);

        var totalPartidas = await _context.Partidas
            .CountAsync(p => p.Fecha >= fechaLimite && p.Resultado != ResultadosPartida.EnProgreso);

        var partidasExitosas = await _context.Partidas
            .CountAsync(p => p.Fecha >= fechaLimite && p.Resultado == ResultadosPartida.Exitoso);

        var partidasConLesionados = await _context.Partidas
            .CountAsync(p => p.Fecha >= fechaLimite && p.Heridas > 50);

        var promedioTiempo = await _context.Partidas
            .Where(p => p.Fecha >= fechaLimite && p.Resultado != ResultadosPartida.EnProgreso)
            .AverageAsync(p => p.TiempoJugado.TotalSeconds);

        // Métricas del Data Warehouse
        var totalMetricas = await _context.MetricasEventos.CountAsync();
        var metricasRecientes = await _context.MetricasEventos
            .Where(m => m.Fecha >= fechaLimite)
            .ToListAsync();

        return new
        {
            Periodo = "Últimos 30 días",
            Partidas = new
            {
                Total = totalPartidas,
                Exitosas = partidasExitosas,
                ConLesionados = partidasConLesionados,
                TasaExito = totalPartidas > 0 ? Math.Round((double)partidasExitosas / totalPartidas * 100, 2) : 0,
                PromedioTiempoSegundos = Math.Round(promedioTiempo, 2)
            },
            DataWarehouse = new
            {
                TotalMetricas = totalMetricas,
                MetricasRecientes = metricasRecientes.Count,
                PromedioContencion = metricasRecientes.Any() ?
                    Math.Round(metricasRecientes.Average(m => m.CotizacionEstragos), 2) : 0
            },
            FechaActualizacion = DateTime.Now
        };
    }

    private static int CalcularContencionEstragos(Partida partida)
    {
        var puntuacion = 0;

        if (partida.Resultado == ResultadosPartida.Exitoso) puntuacion += 30;
        if (partida.UsoUniforme) puntuacion += 25;
        if (partida.UsoAlarma) puntuacion += 20;
        if (partida.RatioExtincion >= 1.0) puntuacion += 15;
        if (partida.UsoInadecuadoExtintores == 0) puntuacion += 10;

        puntuacion -= (partida.Heridas / 10) * 5;

        return Math.Clamp(puntuacion, 0, 100);
    }
}