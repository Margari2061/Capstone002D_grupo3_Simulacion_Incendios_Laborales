using IncediosWebAPI.Model;
using IncediosWebAPI.Model.DataTransfer;
using IncediosWebAPI.Model.Domain;
using Microsoft.EntityFrameworkCore;

namespace IncediosWebAPI.Modules
{
    public static class PartidaModule
    {
        // ==================== PARTIDAS ====================

        // POST /partidas - Crear nueva partida
        public static async Task<IResult> CrearPartida(PartidaCreateDTO partidaDto, IncendioContext context)
        {
            try
            {
                // Validar que el usuario existe
                var usuarioExiste = await context.Usuarios
                    .AnyAsync(u => u.Rut == partidaDto.RutUsuario);

                if (!usuarioExiste)
                {
                    return Results.NotFound($"Usuario con RUT {partidaDto.RutUsuario} no encontrado");
                }
                //==================================================

                // Validar que el nivel existe
                var nivelExiste = await context.Niveles
                    .AnyAsync(n => n.Id == partidaDto.Nivel);

                if (!nivelExiste)
                {
                    return Results.NotFound($"Nivel con ID {partidaDto.Nivel} no encontrado");
                }

                //==================================================

                // Crear nueva partida con estado INICIADA ACTUALIZADO
                var partida = new Partida
                {
                    RutUsuario = partidaDto.RutUsuario,
                    IdNivel = partidaDto.Nivel,  // ← Usar IdNivel según tu modelo actualizado
                    Fecha = DateTime.UtcNow,

                    // Valores iniciales - el jugador empieza con 0 de daño (full vida)
                    TiempoJugado = TimeSpan.Zero,
                    Resultado = 'P', // 'P' = Partida en Progreso
                    FuegosApagados = 0,
                    ExtintoresUsados = 0,
                    UsoInadecuadoExtintores = 0,
                    UsoAlarma = false,
                    UsoUniforme = false,
                    Heridas = 0,        // 0 = Full vida (sin daño)
                    Desasosiego = 0     // 0 = Completamente calmado
                };
                //==================================================

                // Guardar en base de datos
                context.Partidas.Add(partida);
                await context.SaveChangesAsync();

                //==================================================

                // Devolver SOLO el ID de la partida para que el juego lo use
                return Results.Ok(new
                {
                    PartidaId = partida.Id,
                    Mensaje = "Partida iniciada correctamente",
                    FechaInicio = partida.Fecha
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al comenzar partida: {ex.Message}");
            }
        }

        //=======================================================================================================================================
        // ==================== FINALIZAR PARTIDA ====================
        public static async Task<IResult> FinalizarPartida(PartidaFinalizarDTO finalizarDto, IncendioContext context)
        {
            try
            {
                // Buscar la partida
                var partida = await context.Partidas
                    .FirstOrDefaultAsync(p => p.Id == finalizarDto.PartidaId);

                if (partida == null)
                {
                    return Results.NotFound($"Partida con ID {finalizarDto.PartidaId} no encontrada");
                }

                // Verificar que la partida no esté ya finalizada
                if (partida.Resultado != 'P') // 'P' = En progreso
                {
                    return Results.BadRequest("Esta partida ya fue finalizada anteriormente");
                }

                // Actualizar con los resultados finales
                partida.TiempoJugado = TimeSpan.FromSeconds(finalizarDto.TiempoJugadoSegundos);
                partida.Resultado = finalizarDto.Resultado;
                partida.FuegosApagados = finalizarDto.FuegosApagados;
                partida.ExtintoresUsados = finalizarDto.ExtintoresUsados;
                partida.UsoInadecuadoExtintores = finalizarDto.UsoInadecuadoExtintores;
                partida.UsoAlarma = finalizarDto.UsoAlarma;
                partida.UsoUniforme = finalizarDto.UsoUniforme;
                partida.Heridas = finalizarDto.Heridas;           // 0 = Jugador perfecto (sin daño)
                partida.Desasosiego = finalizarDto.Desasosiego;   // 0 = Jugador calmado

                await context.SaveChangesAsync();

                // Calcular eficiencia basada en el daño recibido
                string eficiencia = partida.Heridas == 0 ? "PERFECTA" :
                                  partida.Heridas <= 25 ? "ALTA" :
                                  partida.Heridas <= 50 ? "MEDIA" : "BAJA";

                // Obtener descripción del resultado
                string descripcionResultado = finalizarDto.Resultado switch
                {
                    '0' => "Condiciones Cumplidas",
                    '1' => "Escape seguro",
                    '2' => "Escape inmediato",
                    '3' => "Escape tardío",
                    '4' => "Muerte",
                    'P' => "En progreso",
                    _ => "Desconocido"
                };

                return Results.Ok(new
                {
                    message = "Partida finalizada exitosamente",
                    partidaId = partida.Id,
                    resultado = descripcionResultado,
                    metricas = new
                    {
                        tiempoTotalSegundos = partida.TiempoJugado.TotalSeconds,
                        fuegosApagados = partida.FuegosApagados,
                        extintoresUsados = partida.ExtintoresUsados,
                        usoInadecuadoExtintores = partida.UsoInadecuadoExtintores,
                        usoAlarma = partida.UsoAlarma,
                        usoUniforme = partida.UsoUniforme,
                        ratioExtincion = Math.Round(partida.RatioExtincion, 2),
                        heridas = partida.Heridas,        // 0 = Sin daño (full vida)
                        desasosiego = partida.Desasosiego, // 0 = Completamente calmado
                        eficiencia = eficiencia
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al finalizar partida: {ex.Message}");
            }
        }


        //=======================================================================================================================================
        // ==================== OBTENER PARTIDAS POR USUARIO ====================

        // GET /usuarios/{rut}/partidas - Obtener historial de partidas por usuario 
        public static async Task<IResult> GetPartidasPorUsuario(int rut, IncendioContext context)
        {
            try
            {
                var usuarioExiste = await context.Usuarios
                    .AnyAsync(u => u.Rut == rut);

                if (!usuarioExiste)
                {
                    return Results.NotFound($"Usuario con RUT {rut} no encontrado");
                }

                var partidas = await context.Partidas
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
                        RatioExtincion = Math.Round(p.RatioExtincion, 2),
                        Heridas = p.Heridas,        // 0 = Sin daño
                        Desasosiego = p.Desasosiego, // 0 = Sin estrés
                        Fecha = p.Fecha
                    })
                    .ToListAsync();

                return Results.Ok(new
                {
                    totalPartidas = partidas.Count,
                    partidas = partidas
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al obtener partidas: {ex.Message}");
            }
        }





        // ==================== OBTENER ESTADÍSTICAS GENERALES ====================
        // GET /partidas/estadisticas - Obtener estadísticas generales
        public static async Task<IResult> GetEstadisticasGenerales(IncendioContext context)
        {
            try
            {
                var totalPartidas = await context.Partidas.CountAsync();
                var partidasExitosas = await context.Partidas
                    .CountAsync(p => p.Resultado == '0'); // '0' = Condiciones Cumplidas

                var promedioTiempo = await context.Partidas
                    .AverageAsync(p => p.TiempoJugado.TotalSeconds);

                var promedioFuegosApagados = await context.Partidas
                    .AverageAsync(p => p.FuegosApagados);

                var promedioHeridas = await context.Partidas
                    .AverageAsync(p => p.Heridas);

                var partidasSinDanio = await context.Partidas
                    .CountAsync(p => p.Heridas == 0); // Partidas con 0 de daño

                return Results.Ok(new
                {
                    totalPartidas,
                    partidasExitosas,
                    partidasSinDanio, // Partidas donde el jugador tuvo "full vida"
                    tasaExito = totalPartidas > 0 ? Math.Round((double)partidasExitosas / totalPartidas * 100, 2) : 0,
                    tasaSinDanio = totalPartidas > 0 ? Math.Round((double)partidasSinDanio / totalPartidas * 100, 2) : 0,
                    promedioTiempoSegundos = Math.Round(promedioTiempo, 2),
                    promedioFuegosApagados = Math.Round(promedioFuegosApagados, 2),
                    promedioHeridas = Math.Round(promedioHeridas, 2)
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error al obtener estadísticas: {ex.Message}");
            }
        }
    }
    }
