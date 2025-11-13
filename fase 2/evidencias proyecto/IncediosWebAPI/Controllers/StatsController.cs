using IncediosWebAPI.Extensions;
using IncediosWebAPI.Model;
using IncediosWebAPI.Model.DataTransfer;
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
            _logger.LogInformation(" Calculando KPIs avanzados...");

            // Verificar si hay datos suficientes
            var totalPartidas = await _context.Partidas.CountAsync();
            var totalUsuarios = await _context.Usuarios.CountAsync();

            if (totalPartidas == 0 || totalUsuarios == 0)
            {
                _logger.LogInformation("No hay datos suficientes");
                return Ok();
            }

            // Calcular KPIs con datos reales
            var kpiGenero = await CalcularKPI_VulnerabilidadGenero();
            var kpiEstres = await CalcularKPI_ImpactoEstres();
            var kpiEficiencia = await CalcularKPI_EficienciaExperiencia();

            _logger.LogInformation("KPIs calculados exitosamente con datos reales");

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
            _logger.LogError(ex, " Error en KPIs");
            // Si hay error, devolver datos de ejemplo
            return Ok();
        }
    }

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
                    Mensaje = ""
                },
                ImpactoEstres = new
                {
                    CorrelacionEstresLesiones = 65.8,
                    UmbralEstresCritico = 62.0,
                    TotalMuestras = 189,
                    Interpretacion = "Alta correlación",
                    Mensaje = ""
                },
                EficienciaExperiencia = new
                {
                    VentajaMonitores = 42.3,
                    EficienciaMonitores = 2.1,
                    EficienciaNoMonitores = 1.5,
                    TasaExitoMonitores = 78.5,
                    TasaExitoNoMonitores = 55.2,
                    PartidasAnalizadas = 124,
                    Mensaje = ""
                },
                FechaCalculo = DateTime.Now,
                Modo = "Ejemplo",
                MensajeGlobal = " KPIs de demostración - Listos para datos reales"
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
    // ==================== MÉTODOS PRIVADOS EXISTENTES REALES====================

    private async Task<object> ObtenerEstadisticasETL()
    {
        var fechaLimite = DateTime.UtcNow.AddDays(-30);

        List<Partida> partidas = await _context
            .Partidas
            .Where(p => p.Fecha >= fechaLimite && p.Resultado != ResultadosPartida.EnProgreso)
            .ToListAsync();

        var totalPartidas = partidas
            .Count;

        var partidasExitosas = partidas
            .Where(p => p.Resultado == ResultadosPartida.CondicionesCumplidas)
            .ToList()
            .Count;

        #region Grafico 1
        Dictionary<ResultadosPartida, int> resultados = new()
        {
            {ResultadosPartida.CondicionesCumplidas, partidas.Where(p =>p.Resultado == ResultadosPartida.CondicionesCumplidas).Count() },
            {ResultadosPartida.EscapeSeguro, partidas.Where(p =>p.Resultado == ResultadosPartida.EscapeSeguro).Count() },
            {ResultadosPartida.EscapeInmediato, partidas.Where(p =>p.Resultado == ResultadosPartida.EscapeInmediato).Count() },
            {ResultadosPartida.EscapeTardio, partidas.Where(p =>p.Resultado == ResultadosPartida.EscapeTardio).Count() },
            {ResultadosPartida.Muerte, partidas.Where(p =>p.Resultado == ResultadosPartida.Muerte).Count() },
        };
        #endregion

        double ratioExtincion(IEnumerable<Partida> partidas)
        {
            int fuegos = partidas.Sum(p => p.FuegosApagados);
            int extintores = partidas.Sum(p => p.ExtintoresUsados);

            if (extintores == 0)
                return 0;

            return (double)fuegos / extintores * 10.0;
        }

        var ratioExtincionPorNivel = new double[2];
        ratioExtincionPorNivel[0] = ratioExtincion(partidas.Where(p => p.IdNivel == 1));
        ratioExtincionPorNivel[1] = ratioExtincion(partidas.Where(p => p.IdNivel == 2));

        var partidasConLesionados = partidas
            .Where(p => p.Heridas > 1)
            .ToList()
            .Count;

        var promedioTiempo = partidas
            .Where(p => p.Resultado != ResultadosPartida.EnProgreso)
            .Average(p => p.TiempoJugado.TotalMinutes);

        #region Grafico 2
        Vector[] dataGraf2 = partidas
            .Select(p => new Vector(p.Desasosiego, p.Heridas))
            .ToArray();
        #endregion

        #region Grafico 3
        List<string> fechas = [];
        List<double> tiempoJugadoPorDia = [];
        List<double> tiempoPromedioPorDia = [];
        DateTime startDate = DateTime.UtcNow;
        for(int i = 29; i >= 0; i--)
        {
            DateTime curr = startDate.AddDays(-i);
            fechas.Add($"{curr.Month}-{curr.Day}");

            Partida[] dayRuns = partidas
                .Where(p => p.Fecha.Date == curr.Date)
                .ToArray();

            TimeSpan timePlayedDay = TimeSpan.Zero;
            foreach (Partida r in dayRuns)
                timePlayedDay += r.TiempoJugado;

            tiempoJugadoPorDia.Add(Math.Round(timePlayedDay.TotalMinutes,2));

            if(dayRuns.Length == 0)
            {
                tiempoPromedioPorDia.Add(0.0);
                continue;
            }

            double avgDia = timePlayedDay.TotalMinutes / dayRuns.Length;
            tiempoPromedioPorDia.Add(Math.Round(avgDia,2));
        }
        #endregion

        #region Grafico 4
        int usoAlarma = 0;
        int usoUniforme = 0;
        int sinLesiones = 0;
        int protocoloCompleto = 0;

        foreach(Partida r in partidas)
        {
            bool[] cond = [false, false, false];

            if (r.IdNivel == 2)
            {
                usoAlarma++;
                cond[0] = true;
            }
            else if (r.UsoAlarma)
            {
                usoAlarma++;
                cond[0] = true;
            }

            if (r.UsoUniforme)
            {
                usoUniforme++;
                cond[1] = true;
            }

            if(r.Heridas == 0)
            {
                sinLesiones++;
                cond[2] = true;
            }

            if (cond[0] && cond[1] && cond[2])
                protocoloCompleto++;
        }

        double avgAlarma = Math.Round((double)usoAlarma / totalPartidas * 100.0,2);
        double avgUniforme = Math.Round((double)usoUniforme / totalPartidas * 100.0,2);
        double avgLesiones = Math.Round((double)sinLesiones / totalPartidas * 100.0,2);
        double avgProtocolo = Math.Round((double)protocoloCompleto / totalPartidas * 100.0, 2);
        #endregion


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
                RatioExtincion = ratioExtincionPorNivel,
                ConLesionados = partidasConLesionados,
                TasaExito = totalPartidas > 0 ? Math.Round(partidasExitosas / totalPartidas * 100.0, 2) : 0,
                PromedioTiempoSegundos = Math.Round(promedioTiempo, 2)
            },
            Graf1 = new
            {
                Labels = resultados.Keys.Select(k => k.ToNormal()).ToArray(),
                Values = resultados.Values.ToArray(),
            },
            Graf2 = new
            {
                Data = dataGraf2
            },
            Graf3 = new
            {
                Fechas = fechas,
                TiempoJugado = tiempoJugadoPorDia,
                PromedioJugado = tiempoPromedioPorDia
            },
            Graf4 = new
            {
                Data = new double[] { avgAlarma, avgUniforme, avgLesiones, avgProtocolo }
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