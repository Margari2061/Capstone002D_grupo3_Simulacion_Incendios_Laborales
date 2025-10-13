using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model.IncendioDB.Domain;
using Microsoft.EntityFrameworkCore;

namespace IncediosWebAPI.Modules
{
    public static class ETLModule // --> Va a StatsController
    {
        // ==================== PROCESAR ETL ====================
        //public static async Task<IResult> ProcesarETL(IncendioContext context)
        //{
        //    try
        //    {
        //        // Obtener partidas no procesadas (las de los últimos 30 días por ejemplo)
        //        var fechaLimite = DateTime.UtcNow.AddDays(-30);
        //        var partidasNoProcesadas = await context.Partidas
        //            .Include(p => p.Usuario)
        //            .Include(p => p.Nivel)
        //            .Where(p => p.Fecha >= fechaLimite && p.Resultado != Model.ResultadosPartida.EnProgreso) // Excluir partidas en progreso
        //            .ToListAsync();

        //        if (!partidasNoProcesadas.Any())
        //        {
        //            return Results.Ok(new
        //            {
        //                message = "No hay partidas nuevas para procesar",
        //                partidasProcesadas = 0
        //            });
        //        }

        // Procesar cada partida y crear métricas agregadas
        //var metricasAgregadas = new List<MetricasEvento>();
        //var partidasProcesadasIds = new List<int>();

        //foreach (var partida in partidasNoProcesadas)
        //{
        //    // Crear métrica evento para esta partida
        //    var metricaEvento = new MetricasEvento
        //    {
        //        Fecha = partida.Fecha.Date, // Agrupar por día
        //        ProtocolosSeguidos = partida.Resultado == '0' && partida.UsoUniforme, // Éxito + uniforme
        //        CotizacionEstragos = CalcularContencionEstragos(partida),
        //        Lesionados = partida.Heridas > 50 ? 1 : 0 // Considerar lesionado si tiene más del 50% de daño
        //    };

        //    metricasAgregadas.Add(metricaEvento);
        //    partidasProcesadasIds.Add(partida.Id);
        //}

        // Guardar las métricas en el Data Warehouse (tabla MetricasEvento)
        //        context.MetricasEventos.AddRange(metricasAgregadas);
        //        await context.SaveChangesAsync();



        //        return Results.Ok(new
        //        {
        //            message = "Proceso ETL completado exitosamente",
        //            partidasProcesadas = partidasNoProcesadas.Count,
        //            metricasGeneradas = metricasAgregadas.Count,
        //            fechaProcesamiento = DateTime.UtcNow,
        //            detalle = new
        //            {
        //                partidasExitosas = partidasNoProcesadas.Count(p => p.Resultado == '0'),
        //                partidasConLesionados = partidasNoProcesadas.Count(p => p.Heridas > 50),
        //                promedioRatioExtincion = Math.Round(partidasNoProcesadas.Average(p => p.RatioExtincion), 2)
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Results.Problem($"Error en proceso ETL: {ex.Message}");
        //    }
        //}

        // ==================== OBTENER ESTADÍSTICAS ETL ====================
        //        public static async Task<IResult> GetEstadisticasETL(IncendioContext context)
        //        {
        //            try
        //            {
        //                // Estadísticas de partidas recientes (últimos 30 días)
        //                var fechaLimite = DateTime.UtcNow.AddDays(-30);

        //                var totalPartidas = await context.Partidas
        //                    .CountAsync(p => p.Fecha >= fechaLimite && p.Resultado != 'P');

        //                var partidasExitosas = await context.Partidas
        //                    .CountAsync(p => p.Fecha >= fechaLimite && p.Resultado == '0');

        //                var partidasConLesionados = await context.Partidas
        //                    .CountAsync(p => p.Fecha >= fechaLimite && p.Heridas > 50);

        //                var promedioTiempo = await context.Partidas
        //                    .Where(p => p.Fecha >= fechaLimite && p.Resultado != 'P')
        //                    .AverageAsync(p => p.TiempoJugado.TotalSeconds);

        //                // Métricas del Data Warehouse
        //                var totalMetricas = await context.MetricasEventos.CountAsync();
        //                var metricasRecientes = await context.MetricasEventos
        //                    .Where(m => m.Fecha >= fechaLimite)
        //                    .ToListAsync();

        //                return Results.Ok(new
        //                {
        //                    periodo = "Últimos 30 días",
        //                    partidas = new
        //                    {
        //                        total = totalPartidas,
        //                        exitosas = partidasExitosas,
        //                        conLesionados = partidasConLesionados,
        //                        tasaExito = totalPartidas > 0 ? Math.Round((double)partidasExitosas / totalPartidas * 100, 2) : 0,
        //                        promedioTiempoSegundos = Math.Round(promedioTiempo, 2)
        //                    },
        //                    dataWarehouse = new
        //                    {
        //                        totalMetricas,
        //                        metricasRecientes = metricasRecientes.Count,
        //                        promedioContencion = metricasRecientes.Any() ?
        //                            Math.Round(metricasRecientes.Average(m => m.ContencionEstragos), 2) : 0
        //                    }
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                return Results.Problem($"Error al obtener estadísticas ETL: {ex.Message}");
        //            }
        //        }

        //        // ==================== MÉTODOS AUXILIARES ====================

        //        private static int CalcularContencionEstragos(Partida partida)
        //        {
        //            // Calcular porcentaje de contención basado en múltiples factores
        //            var puntuacion = 0;

        //            // +30 puntos por éxito en la partida
        //            if (partida.Resultado == '0') puntuacion += 30;

        //            // +25 puntos por usar uniforme
        //            if (partida.UsoUniforme) puntuacion += 25;

        //            // +20 puntos por usar alarma
        //            if (partida.UsoAlarma) puntuacion += 20;

        //            // +15 puntos por buen ratio de extinción (> 1.0)
        //            if (partida.RatioExtincion >= 1.0) puntuacion += 15;

        //            // +10 puntos por pocos errores con extintores
        //            if (partida.UsoInadecuadoExtintores == 0) puntuacion += 10;

        //            // -5 puntos por cada 10% de daño recibido
        //            puntuacion -= (partida.Heridas / 10) * 5;

        //            // Asegurar que esté entre 0-100
        //            return Math.Clamp(puntuacion, 0, 100);
        //        }

        //        // ==================== LIMPIAR DATOS ANTIGUOS ====================
        //        public static async Task<IResult> LimpiarDatosAntiguos(IncendioContext context)
        //        {
        //            try
        //            {
        //                // Eliminar partidas muy antiguas (más de 1 año) para mantener la BD limpia
        //                var fechaLimite = DateTime.UtcNow.AddYears(-1);
        //                var partidasAntiguas = await context.Partidas
        //                    .Where(p => p.Fecha < fechaLimite)
        //                    .ToListAsync();

        //                var cantidadEliminadas = partidasAntiguas.Count;

        //                if (partidasAntiguas.Any())
        //                {
        //                    context.Partidas.RemoveRange(partidasAntiguas);
        //                    await context.SaveChangesAsync();
        //                }

        //                return Results.Ok(new
        //                {
        //                    message = "Limpieza de datos antiguos completada",
        //                    partidasEliminadas = cantidadEliminadas,
        //                    fechaLimite = fechaLimite.ToString("yyyy-MM-dd")
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                return Results.Problem($"Error en limpieza de datos: {ex.Message}");
        //            }
        //        }
        //    }
    }
}