
// CONFIGURA LA BASE DE DATOS
//Comandos: add-migration {NombreMigracion} ... para agregar cambios a base de datos
// update-database ... para actualizar la base de datos

using IncediosWebAPI.Model.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace IncediosWebAPI.Model;

public class IncendioContext : DbContext
{

    public IncendioContext(DbContextOptions<IncendioContext> options) : base(options) { }


    // DBSets
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Sede> Sedes { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Partida> Partidas { get; set; }
    public DbSet<Nivel> Niveles { get; set; }
    public DbSet<MetricasEvento> MetricasEventos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        //SetEmpresa(modelBuilder);

        // Configurar relaciones ------------------------


        //--------- Configurar propiedades de Strings ------------------
        modelBuilder.Entity<Empresa>()
                .Property(e => e.Nombre)
                ;

        modelBuilder.Entity<Empresa>()
                .Property(e => e.Rubro)
                ;

        modelBuilder.Entity<Usuario>()
                .Property(u => u.Nombre)
                ;

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Mail)
            ;

        // Configurar valores por defecto
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Genero)
            .HasDefaultValue('N');

        modelBuilder.Entity<Usuario>()
            .Property(u => u.EsMonitor)
            .HasDefaultValue(false);

        modelBuilder.Entity<Partida>()
            .Property(p => p.Fecha)
            .HasDefaultValueSql("GETUTCDATE()");
        // ==========================================================================================================

        //  DATOS INICIALES DE NIVELES (AGREGAR ESTO)
        modelBuilder.Entity<Nivel>().HasData(
            new Nivel
            {
                Id = 1,
                Nombre = "Incendio Oficina Nivel 1",
                FuegosApagadosEsperados = 2,
                ExtintoresUsadosEsperados = 1,
                UsoAlarmaEsperado = true
            },
            new Nivel
            {
                Id = 2,
                Nombre = "Fuego Cocina",
                FuegosApagadosEsperados = 3,
                ExtintoresUsadosEsperados = 2,
                UsoAlarmaEsperado = false
            }
        );
    }





    private void SetEmpresa(ModelBuilder builder)
    {
        EntityTypeBuilder<Empresa> empresa = builder.Entity<Empresa>();

        Empresa[] empresas = [
            new() { Rut = 11111111, Dv='1', Nombre="Empresa Falsa", Rubro="Mentir"}
            ];

        empresa.HasData(empresas);
    }
}