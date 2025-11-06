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
                    ProtocolosSeguidos = partida.Resultado == ResultadosPartida.CondicionesCumplidas && partida.UsoUniforme,
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


    //====================================================================================================
    // ==================== NUEVO: API PARA KPIs AVANZADOS ====================
    [HttpGet("api/kpis/avanzados")]
    public async Task<IActionResult> GetKPIsAvanzados()
    {
        try
        {
            _logger.LogInformation("🔍 Calculando KPIs avanzados...");

            // Verificar si hay datos suficientes
            var totalPartidas = await _context.Partidas.CountAsync();
            var totalUsuarios = await _context.Usuarios.CountAsync();

            if (totalPartidas == 0 || totalUsuarios == 0)
            {
                _logger.LogInformation("📊 No hay datos suficientes, generando KPIs de ejemplo");
                return Ok(GenerarKPIsDeEjemplo());
            }

            // Calcular KPIs con datos reales
            var kpiGenero = await CalcularKPI_VulnerabilidadGenero();
            var kpiEstres = await CalcularKPI_ImpactoEstres();
            var kpiEficiencia = await CalcularKPI_EficienciaExperiencia();

            _logger.LogInformation("✅ KPIs calculados exitosamente con datos reales");

            return Ok(new
            {
                VulnerabilidadGenero = kpiGenero,
                ImpactoEstres = kpiEstres,
                EficienciaExperiencia = kpiEficiencia,
                FechaCalculo = DateTime.Now,
                Modo = "Datos Reales",
                TotalPartidas = totalPartidas,
                TotalUsuarios = totalUsuarios
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error en KPIs, generando ejemplo");
            // Si hay error, devolver datos de ejemplo
            return Ok(GenerarKPIsDeEjemplo());
        }
    }

    // Método para generar KPIs de ejemplo
    private object GenerarKPIsDeEjemplo()
    {
        var random = new Random();

        return new
        {
            VulnerabilidadGenero = new
            {
                BrechaPorcentaje = Math.Round((random.NextDouble() * 30) - 15, 2), // Entre -15% y +15%
                TasaHombres = Math.Round(65 + random.NextDouble() * 20, 2), // 65-85%
                TasaMujeres = Math.Round(65 + random.NextDouble() * 20, 2), // 65-85%
                GeneroMasEfectivo = random.Next(2) == 0 ? "Mujeres" : "Hombres",
                TotalMuestras = random.Next(50, 200),
                Mensaje = "📊 Datos de ejemplo - Esperando datos reales"
            },
            ImpactoEstres = new
            {
                CorrelacionEstresLesiones = Math.Round(random.NextDouble() * 100, 2), // 0-100%
                UmbralEstresCritico = Math.Round(50 + random.NextDouble() * 30, 2), // 50-80
                TotalMuestras = random.Next(100, 300),
                Interpretacion = random.Next(3) switch
                {
                    0 => "Alta correlación",
                    1 => "Correlación moderada",
                    _ => "Baja correlación"
                },
                Mensaje = "😰 Datos de ejemplo - Esperando datos reales"
            },
            EficienciaExperiencia = new
            {
                VentajaMonitores = Math.Round(random.NextDouble() * 60, 2), // 0-60%
                EficienciaMonitores = Math.Round(1.5 + random.NextDouble() * 1.5, 2), // 1.5-3.0
                EficienciaNoMonitores = Math.Round(1.0 + random.NextDouble() * 1.0, 2), // 1.0-2.0
                TasaExitoMonitores = Math.Round(70 + random.NextDouble() * 20, 2), // 70-90%
                TasaExitoNoMonitores = Math.Round(50 + random.NextDouble() * 30, 2), // 50-80%
                PartidasAnalizadas = random.Next(80, 150),
                Mensaje = "⚡ Datos de ejemplo - Esperando datos reales"
            },
            FechaCalculo = DateTime.Now,
            Modo = "Ejemplo",
            MensajeGlobal = "🎯 Estos son KPIs de ejemplo. Los datos reales aparecerán cuando se jueguen partidas."
        };
    }


    //=================== BORRAR POR TESTEO =================================================================

    // ==================== NUEVO ENDPOINT SIMPLE PARA TESTING ====================
    [HttpGet("api/kpis/test")]
    public IActionResult GetKPIsTest()
    {
        try
        {
            var random = new Random();

            var kpis = new
            {
                VulnerabilidadGenero = new
                {
                    BrechaPorcentaje = 12.5,
                    TasaHombres = 68.2,
                    TasaMujeres = 76.7,
                    GeneroMasEfectivo = "Mujeres",
                    TotalMuestras = 142,
                    Mensaje = "📊 Datos de ejemplo"
                },
                ImpactoEstres = new
                {
                    CorrelacionEstresLesiones = 65.8,
                    UmbralEstresCritico = 62.0,
                    TotalMuestras = 189,
                    Interpretacion = "Alta correlación",
                    Mensaje = "😰 Datos de ejemplo"
                },
                EficienciaExperiencia = new
                {
                    VentajaMonitores = 42.3,
                    EficienciaMonitores = 2.1,
                    EficienciaNoMonitores = 1.5,
                    TasaExitoMonitores = 78.5,
                    TasaExitoNoMonitores = 55.2,
                    PartidasAnalizadas = 124,
                    Mensaje = "⚡ Datos de ejemplo"
                },
                FechaCalculo = DateTime.Now,
                Modo = "Ejemplo",
                MensajeGlobal = "🎯 KPIs de demostración - Listos para datos reales"
            };

            return Ok(kpis);
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                error = "Error mínimo: " + ex.Message,
                kpisDeEmergencia = new
                {
                    mensaje = "KPIs básicos funcionando",
                    fecha = DateTime.Now
                }
            });
        }
    }














    // ==============================================================================================================
    // ==================== MÉTODOS PRIVADOS EXISTENTES ====================

    private async Task<object> ObtenerEstadisticasETL()
    {
        var fechaLimite = DateTime.UtcNow.AddDays(-30);

        List<Partida> partidas = await _context
            .Partidas
            .Where(p => p.Fecha >= fechaLimite)
            .ToListAsync();

        var totalPartidas = partidas
            .Where(p => p.Resultado != ResultadosPartida.EnProgreso)
            .ToList()
            .Count;

        var partidasExitosas = partidas
            .Where(p => p.Resultado == ResultadosPartida.CondicionesCumplidas)
            .ToList()
            .Count;

        var partidasConLesionados = partidas
            .Where(p => p.Heridas > 1)
            .ToList()
            .Count;

        var promedioTiempo = partidas
            .Average(p => p.TiempoJugado.TotalMinutes);

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
                TasaExito = totalPartidas > 0 ? Math.Round(partidasExitosas / totalPartidas * 100.0, 2) : 0,
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

    // CORRECCIÓN: Método sin RatioExtincion
    private static int CalcularContencionEstragos(Partida partida)
    {
        var puntuacion = 0;

        // +30 puntos por éxito en la partida
        if (partida.Resultado == ResultadosPartida.CondicionesCumplidas)
            puntuacion += 30;

        // +25 puntos por usar uniforme
        if (partida.UsoUniforme)
            puntuacion += 25;

        // +20 puntos por usar alarma
        if (partida.UsoAlarma)
            puntuacion += 20;

        // +15 puntos por buen uso de extintores (pocos usos inadecuados)
        if (partida.UsoInadecuadoExtintores <= 1)
            puntuacion += 15;

        // +10 puntos por apagar muchos fuegos
        if (partida.FuegosApagados >= 3)
            puntuacion += 10;

        // -5 puntos por cada 10% de daño recibido
        puntuacion -= (partida.Heridas / 10) * 5;

        // Asegurar que esté entre 0-100
        return Math.Clamp(puntuacion, 0, 100);
    }

    // ==================== NUEVOS MÉTODOS PARA KPIs ====================

    private async Task<object> CalcularKPI_VulnerabilidadGenero()
    {
        var resultados = await _context.Partidas
            .Include(p => p.Usuario)
            .Where(p => p.Resultado != ResultadosPartida.EnProgreso)
            .GroupBy(p => p.Usuario.Genero)
            .Select(g => new
            {
                Genero = g.Key.ToString(),
                TotalPartidas = g.Count(),
                ProtocolosSeguidos = g.Count(p => p.UsoUniforme && p.UsoAlarma),
                TasaProtocolos = (double)g.Count(p => p.UsoUniforme && p.UsoAlarma) / g.Count() * 100
            })
            .ToListAsync();

        var hombres = resultados.FirstOrDefault(r => r.Genero == "M");
        var mujeres = resultados.FirstOrDefault(r => r.Genero == "F");

        double brecha = 0;
        if (hombres != null && mujeres != null && hombres.TasaProtocolos > 0)
        {
            brecha = (mujeres.TasaProtocolos - hombres.TasaProtocolos) / hombres.TasaProtocolos * 100;
        }

        return new
        {
            BrechaPorcentaje = Math.Round(brecha, 2),
            TasaHombres = Math.Round(hombres?.TasaProtocolos ?? 0, 2),
            TasaMujeres = Math.Round(mujeres?.TasaProtocolos ?? 0, 2),
            GeneroMasEfectivo = brecha > 0 ? "Mujeres" : "Hombres",
            TotalMuestras = (hombres?.TotalPartidas ?? 0) + (mujeres?.TotalPartidas ?? 0)
        };
    }

    private async Task<object> CalcularKPI_ImpactoEstres()
    {
        var datos = await _context.Partidas
            .Where(p => p.Resultado != ResultadosPartida.EnProgreso)
            .Select(p => new { p.Desasosiego, p.Heridas })
            .ToListAsync();

        if (!datos.Any() || datos.Count < 2)
            return new
            {
                CorrelacionEstresLesiones = 0,
                UmbralEstresCritico = 0,
                TotalMuestras = 0,
                Mensaje = "Datos insuficientes para cálculo"
            };

        // Cálculo simplificado de correlación
        double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;
        int n = datos.Count;

        foreach (var dato in datos)
        {
            sumX += dato.Desasosiego;
            sumY += dato.Heridas;
            sumXY += dato.Desasosiego * dato.Heridas;
            sumX2 += dato.Desasosiego * dato.Desasosiego;
            sumY2 += dato.Heridas * dato.Heridas;
        }

        double correlacion = (n * sumXY - sumX * sumY) /
                           Math.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY));

        // Umbral donde heridas son significativas (>25)
        var umbral = datos
            .Where(d => d.Heridas > 25)
            .Select(d => d.Desasosiego)
            .DefaultIfEmpty(0)
            .Average();

        return new
        {
            CorrelacionEstresLesiones = Math.Round(Math.Abs(correlacion) * 100, 2),
            UmbralEstresCritico = Math.Round(umbral, 2),
            TotalMuestras = datos.Count,
            Interpretacion = correlacion > 0.5 ? "Alta correlación" :
                           correlacion > 0.3 ? "Correlación moderada" : "Baja correlación"
        };
    }

    private async Task<object> CalcularKPI_EficienciaExperiencia()
    {
        var eficiencia = await _context.Partidas
            .Include(p => p.Usuario)
            .Where(p => p.Resultado != ResultadosPartida.EnProgreso && p.ExtintoresUsados > 0)
            .GroupBy(p => p.Usuario.EsMonitor)
            .Select(g => new
            {
                EsMonitor = g.Key,
                EficienciaPromedio = g.Average(p => (double)p.FuegosApagados / p.ExtintoresUsados),
                TotalPartidas = g.Count(),
                TasaExito = g.Count(p => p.Resultado == ResultadosPartida.CondicionesCumplidas) * 100.0 / g.Count()
            })
            .ToListAsync();

        var monitores = eficiencia.FirstOrDefault(e => e.EsMonitor);
        var noMonitores = eficiencia.FirstOrDefault(e => !e.EsMonitor);

        double ventaja = 0;
        if (monitores != null && noMonitores != null && noMonitores.EficienciaPromedio > 0)
        {
            ventaja = (monitores.EficienciaPromedio - noMonitores.EficienciaPromedio) / noMonitores.EficienciaPromedio * 100;
        }

        return new
        {
            VentajaMonitores = Math.Round(ventaja, 2),
            EficienciaMonitores = Math.Round(monitores?.EficienciaPromedio ?? 0, 2),
            EficienciaNoMonitores = Math.Round(noMonitores?.EficienciaPromedio ?? 0, 2),
            TasaExitoMonitores = Math.Round(monitores?.TasaExito ?? 0, 2),
            TasaExitoNoMonitores = Math.Round(noMonitores?.TasaExito ?? 0, 2),
            PartidasAnalizadas = (monitores?.TotalPartidas ?? 0) + (noMonitores?.TotalPartidas ?? 0)
        };
    }
}