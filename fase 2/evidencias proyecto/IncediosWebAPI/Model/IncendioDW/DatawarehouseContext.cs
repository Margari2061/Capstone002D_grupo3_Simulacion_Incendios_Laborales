using IncediosWebAPI.Model.IncendioDW.Dimensiones;
using IncediosWebAPI.Model.IncendioDB.Domain;
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
                entity.HasIndex(e => new { e.Ano, e.Mes }); // agrupamientos por mes
                entity.HasIndex(e => new { e.Ano, e.Trimestre }); // trimestres
                entity.HasIndex(e => e.EsFinDeSemana); // análisis de fines de semana
            });


        }
    }
