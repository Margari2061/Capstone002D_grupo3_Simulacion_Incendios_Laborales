
// CONFIGURA LA BASE DE DATOS
//Comandos: add-migration {NombreMigracion} ... para agregar cambios a base de datos
// update-database ... para actualizar la base de datos

using IncediosWebAPI.Model.IncendioDB.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace IncediosWebAPI.Model.IncendioDB;

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

        //  DATOS INICIALES DE NIVELES (AGREGAR ESTO)
        Empresa[] empresas =
        [
            new()
            {
                Rut = 90306491,
                Dv = '9',
                Nombre = "Clinica Andes",
                Rubro = "Medicina",
            },
            //new()
            //{
            //    Rut = 90306491,
            //    Dv = '9',
            //    Nombre = "Clinica Andes",
            //    Rubro = "Medicina",
            //},
            //new()
            //{
            //    Rut = 90306491,
            //    Dv = '9',
            //    Nombre = "Clinica Andes",
            //    Rubro = "Medicina",
            //},
        ];

        modelBuilder.Entity<Empresa>().HasData(empresas);


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
}
