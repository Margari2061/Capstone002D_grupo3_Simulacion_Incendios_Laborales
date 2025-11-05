using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model.IncendioDB.Domain;
using IncediosWebAPI.Model.IncendioDW.Dimensiones;
using Microsoft.EntityFrameworkCore;

namespace IncediosWebAPI.Model.IncendioDW
{
    public class DatawarehouseContext : DbContext
    {
        public DatawarehouseContext(DbContextOptions<DatawarehouseContext> options) : base(options) { }

        // ==================== DIMENSIONES ====================
        public DbSet<TiempoDW> Tiempos { get; set; }
        public DbSet<SedeDW> Sedes { get; set; }
        public DbSet<DepartamentoDW> Departamentos { get; set; }
        public DbSet<UsuarioDW> Usuarios { get; set; }
        public DbSet<PartidaDW> Partidas { get; set; }

        // ==================== HECHOS (FACT TABLES) ====================
        public DbSet<MetricasEvento> MetricasEventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== CONFIGURACIÓN DE DIMENSIONES ====================

            // TiempoDW - Dimensión Temporal
            modelBuilder.Entity<TiempoDW>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Fecha).IsUnique();
                entity.HasIndex(e => new { e.Ano, e.Mes });
                entity.HasIndex(e => new { e.Ano, e.Trimestre });
                entity.HasIndex(e => e.EsFinDeSemana);
            });

            // SedeDW - Dimensión Sede
            modelBuilder.Entity<SedeDW>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Nombre);
                entity.HasIndex(e => e.Region);
                entity.HasIndex(e => e.EmpresaNombre);
            });

            // DepartamentoDW - Dimensión Departamento
            modelBuilder.Entity<DepartamentoDW>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Nombre);
                entity.HasIndex(e => e.SedeId);
                entity.HasIndex(e => e.SedeNombre);
                entity.HasIndex(e => e.TasaExitoPromedio);
            });

            // UsuarioDW - Dimensión Usuario
            modelBuilder.Entity<UsuarioDW>(entity =>
            {
                entity.HasKey(e => e.Rut);
                entity.HasIndex(e => e.Nombre);
                entity.HasIndex(e => e.DepartamentoId);
                entity.HasIndex(e => e.Genero);
                entity.HasIndex(e => e.EsMonitor);
                entity.HasIndex(e => e.EsPlayer);
                entity.HasIndex(e => e.EsAdmin);
                entity.HasIndex(e => e.TasaExito);
                entity.HasIndex(e => e.Edad);
            });

            // PartidaDW - Dimensión Partida 
            modelBuilder.Entity<PartidaDW>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UsuarioRut);
                entity.HasIndex(e => e.TiempoId);
                entity.HasIndex(e => e.NivelId);
                entity.HasIndex(e => e.Resultado);
                entity.HasIndex(e => e.PuntuacionContencion);
                entity.HasIndex(e => e.ProtocolosSeguidos);
                entity.HasIndex(e => new { e.TiempoId, e.Resultado });
            });

            // ==================== CONFIGURACIÓN DE HECHOS ====================

            // MetricasEvento - Tabla de Hechos Principal (modelo físico existente)
            modelBuilder.Entity<MetricasEvento>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Índices para optimización de consultas
                entity.HasIndex(e => e.Fecha);
                entity.HasIndex(e => e.ProtocolosSeguidos);
                entity.HasIndex(e => e.CotizacionEstragos);
                entity.HasIndex(e => e.Lesionados);
                entity.HasIndex(e => new { e.Fecha, e.ProtocolosSeguidos });

                // NOTA: Las FKs a dimensiones se manejarán a nivel de aplicación
                // ya que MetricasEvento es el modelo físico existente
            });

            // ==================== RELACIONES ENTRE DIMENSIONES ====================

        //    // Sede -> Departamentos (1 a N)
        //    modelBuilder.Entity<SedeDW>()
        //        .HasMany(s => s.Departamentos)
        //        .WithOne()
        //        .HasForeignKey(d => d.SedeId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // Departamento -> Usuarios (1 a N)
        //    modelBuilder.Entity<DepartamentoDW>()
        //        .HasMany(d => d.Usuarios)
        //        .WithOne()
        //        .HasForeignKey(u => u.DepartamentoId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // Usuario -> Partidas (1 a N)
        //    modelBuilder.Entity<UsuarioDW>()
        //        .HasMany(u => u.Partidas)
        //        .WithOne(p => p.Usuario)
        //        .HasForeignKey(p => p.UsuarioRut)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // Tiempo -> Partidas (1 a N)
        //    modelBuilder.Entity<TiempoDW>()
        //        .HasMany<TiempoDW>()
        //        .WithOne()
        //        .HasForeignKey(p => p.TiempoId)
        //        .OnDelete(DeleteBehavior.Restrict);
        }

        // ==================== MÉTODOS DE UTILIDAD ====================

        /// <summary>
        /// Obtiene o crea una dimensión de tiempo
        /// </summary>
        public async Task<TiempoDW> ObtenerOCrearTiempo(DateTime fecha)
        {
            var tiempoId = TiempoDW.GenerarId(fecha);
            var tiempo = await Tiempos.FindAsync(tiempoId);

            if (tiempo == null)
            {
                tiempo = new TiempoDW(fecha);
                Tiempos.Add(tiempo);
                await SaveChangesAsync();
            }

            return tiempo;
        }

        /// <summary>
        /// Ejecuta proceso ETL para poblar dimensiones desde datos operacionales
        /// </summary>
        public async Task EjecutarETL(IncendioContext contextoOperacional)
        {
            // 1. Poblar dimensión Tiempo
            await PoblarDimensionTiempo(contextoOperacional);

            // 2. Poblar dimensión Usuario
            await PoblarDimensionUsuario(contextoOperacional);

            // 3. Poblar dimensión Partida
            await PoblarDimensionPartida(contextoOperacional);

            // 4. Recalcular métricas agregadas
            await MantenimientoDW();
        }

        private async Task PoblarDimensionTiempo(IncendioContext contextoOperacional)
        {
            // Obtener todas las fechas únicas de partidas y métricas
            var fechasPartidas = await contextoOperacional.Partidas
                .Select(p => p.Fecha.Date)
                .Distinct()
                .ToListAsync();

            var fechasMetricas = await contextoOperacional.MetricasEventos
                .Select(m => m.Fecha.DateTime.Date)
                .Distinct()
                .ToListAsync();

            var todasFechas = fechasPartidas.Union(fechasMetricas).Distinct();

            foreach (var fecha in todasFechas)
            {
                await ObtenerOCrearTiempo(fecha);
            }
        }

        private async Task PoblarDimensionUsuario(IncendioContext contextoOperacional)
        {
            // Implementar población de usuarios desde operacional
        }

        private async Task PoblarDimensionPartida(IncendioContext contextoOperacional)
        {
            // Implementar población de partidas desde operacional
        }

        /// <summary>
        /// Ejecuta limpieza y optimización del Data Warehouse
        /// </summary>
        public async Task MantenimientoDW()
        {
            await RecalcularMetricasUsuarios();
            await RecalcularMetricasDepartamentos();
        }

        private async Task RecalcularMetricasUsuarios()
        {
            // Lógica para recalcular métricas de usuarios
            var usuariosConMetricas = await Usuarios
                .Where(u => u.TotalPartidas > 0)
                .ToListAsync();

            foreach (var usuario in usuariosConMetricas)
            {
                var partidasUsuario = await Partidas
                    .Where(p => p.UsuarioRut == usuario.Rut)
                    .ToListAsync();

                usuario.TotalPartidas = partidasUsuario.Count;
                usuario.PartidasExitosas = partidasUsuario.Count(p => p.Resultado == "CondicionesCumplidas");
                usuario.TasaExito = usuario.TotalPartidas > 0 ?
                    (double)usuario.PartidasExitosas / usuario.TotalPartidas * 100 : 0;
                usuario.TiempoPromedioPartida = partidasUsuario.Any() ?
                    partidasUsuario.Average(p => p.TiempoJugadoSegundos) : 0;
            }

            await SaveChangesAsync();
        }

        private async Task RecalcularMetricasDepartamentos()
        {
            // Lógica para recalcular métricas de departamentos
            var departamentos = await Departamentos.ToListAsync();

            foreach (var depto in departamentos)
            {
                var usuariosDepto = await Usuarios
                    .Where(u => u.DepartamentoId == depto.Id)
                    .ToListAsync();

                depto.TotalUsuarios = usuariosDepto.Count;
                depto.TotalPartidas = usuariosDepto.Sum(u => u.TotalPartidas);
                depto.TasaExitoPromedio = usuariosDepto.Any(u => u.TotalPartidas > 0) ?
                    usuariosDepto.Where(u => u.TotalPartidas > 0).Average(u => u.TasaExito) : 0;
            }

            await SaveChangesAsync();
        }
    }
}