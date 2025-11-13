using IncediosWebAPI.Model;
using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model.IncendioDB.Domain;
using IncediosWebAPI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncediosWebAPI.Controllers;

[WebAuthorize(AppRoles.Admin)]
public class InicioController : Controller
{
    private readonly IncendioContext _context;

    public InicioController(IncendioContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return RedirectToAction("estadisticas", "Stats");
    }

    // ==================== API PARA DATOS EN TIEMPO REAL ====================
    [HttpGet]
    public async Task<IActionResult> ObtenerDatosActualizados()
    {
        try
        {
            var datos = await ObtenerDatosDashboard();
            return Ok(datos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error al cargar datos" });
        }
    }

    // ==================== MÉTODOS PRIVADOS ====================

    private async Task<DashboardViewModel> ObtenerDatosDashboard()
    {
        var fechaHace30Dias = DateTime.UtcNow.AddDays(-30);

        // Estadísticas de usuarios
        var totalUsuarios = await _context.Usuarios.CountAsync();
        var usuariosActivos = await _context.Usuarios
            .CountAsync(u => u.Roles != AppRoles.None);
        var nuevosUsuarios = await _context.Usuarios
            .CountAsync(u => u.Roles != AppRoles.None); // Podrías agregar campo FechaRegistro

        // Estadísticas de partidas
        var totalPartidas = await _context.Partidas.CountAsync();
        var partidasRecientes = await _context.Partidas
            .CountAsync(p => p.Fecha >= fechaHace30Dias);
        var partidasExitosas = await _context.Partidas
            .CountAsync(p => p.Resultado == ResultadosPartida.CondicionesCumplidas);
        var partidasEnProgreso = await _context.Partidas
            .CountAsync(p => p.Resultado == ResultadosPartida.EnProgreso);

        // Métricas del Data Warehouse
        var totalMetricas = await _context.MetricasEventos.CountAsync();
        var metricasRecientes = await _context.MetricasEventos
            .Where(m => m.Fecha >= fechaHace30Dias)
            .ToListAsync();

        // Usuarios por departamento
        var usuariosPorDepartamento = await _context.Usuarios
            .Include(u => u.Departamento)
            .Where(u => u.Roles != AppRoles.None)
            .GroupBy(u => u.Departamento!.Nombre)
            .Select(g => new
            {
                Departamento = g.Key,
                Cantidad = g.Count()
            })
            .ToListAsync();

        // Partidas recientes para el timeline
        var partidasRecientesDetalle = await _context.Partidas
            .Include(p => p.Usuario)
            .Include(p => p.Nivel)
            .Where(p => p.Fecha >= fechaHace30Dias)
            .OrderByDescending(p => p.Fecha)
            .Take(10)
            .Select(p => new
            {
                p.Id,
                Usuario = p.Usuario!.Nombre,
                Nivel = p.Nivel!.Nombre,
                p.Resultado,
                p.Fecha,
                p.TiempoJugado,
                p.Heridas
            })
            .ToListAsync();

        return new DashboardViewModel
        {
            // Resumen general
            TotalUsuarios = totalUsuarios,
            UsuariosActivos = usuariosActivos,
            TotalPartidas = totalPartidas,
            PartidasRecientes = partidasRecientes,
            PartidasExitosas = partidasExitosas,
            PartidasEnProgreso = partidasEnProgreso,
            TotalMetricas = totalMetricas,

            // Estadísticas calculadas
            TasaExito = totalPartidas > 0 ? Math.Round((double)partidasExitosas / totalPartidas * 100, 1) : 0,
            PromedioTiempoPartida = partidasRecientes > 0 ?
                Math.Round(await _context.Partidas
                    .Where(p => p.Fecha >= fechaHace30Dias && p.Resultado != ResultadosPartida.EnProgreso)
                    .AverageAsync(p => p.TiempoJugado.TotalMinutes), 1) : 0,

            // Data Warehouse
            MetricasRecientes = metricasRecientes.Count,
            PromedioContencion = metricasRecientes.Any() ?
                Math.Round(metricasRecientes.Average(m => m.CotizacionEstragos), 1) : 0,
            TotalProtocolosSeguidos = metricasRecientes.Count(m => m.ProtocolosSeguidos),

            // Datos para gráficos
            UsuariosPorDepartamento = usuariosPorDepartamento,
            PartidasRecientesDetalle = partidasRecientesDetalle,

            // Información de sistema
            FechaActualizacion = DateTime.Now,
            PeriodoAnalisis = "Últimos 30 días"
        };
    }
}

// ==================== VIEW MODEL PARA EL DASHBOARD ====================
public class DashboardViewModel
{
    // Resumen general
    public int TotalUsuarios { get; set; }
    public int UsuariosActivos { get; set; }
    public int TotalPartidas { get; set; }
    public int PartidasRecientes { get; set; }
    public int PartidasExitosas { get; set; }
    public int PartidasEnProgreso { get; set; }
    public int TotalMetricas { get; set; }

    // Estadísticas
    public double TasaExito { get; set; }
    public double PromedioTiempoPartida { get; set; }

    // Data Warehouse
    public int MetricasRecientes { get; set; }
    public double PromedioContencion { get; set; }
    public int TotalProtocolosSeguidos { get; set; }

    // Datos para gráficos
    public object? UsuariosPorDepartamento { get; set; }
    public object? PartidasRecientesDetalle { get; set; }

    // Metadata
    public DateTime FechaActualizacion { get; set; }
    public string PeriodoAnalisis { get; set; } = string.Empty;
}