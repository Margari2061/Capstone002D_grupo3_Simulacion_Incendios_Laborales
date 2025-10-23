
// CONFIGURA LA BASE DE DATOS
//Comandos: add-migration {NombreMigracion} ... para agregar cambios a base de datos
// update-database ... para actualizar la base de datos

using IncediosWebAPI.Extensions;
using IncediosWebAPI.Model.IncendioDB.Domain;
using Microsoft.EntityFrameworkCore;

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
            }
        ];

        modelBuilder.Entity<Empresa>().HasData(empresas);

        Sede[] sedes = 
        [
            new()
            {
                Id = 1, 
                RutEmpresa=90306491, 
                Nombre="UCI",
                Comuna="Hualpén",
                Region="Bío-Bío",
                Direccion="Calle Siempreviva 123"
            }
        ];

        modelBuilder.Entity<Sede>().HasData(sedes);

        Departamento[] departamentos =
        [
            new()
            {
                Id = 1,
                IdSede=1,
                Nombre ="UCI"
            },
            new()
            {
                Id = 2,
                IdSede=1,
                Nombre ="Neurología"
            },
            new()
            {
                Id = 3,
                IdSede=1,
                Nombre ="IT"
            },
            new()
            {
                Id = 4,
                IdSede=1,
                Nombre ="Laboratorios"
            },
            new()
            {
                Id = 5,
                IdSede=1,
                Nombre ="Toma de Muestras"
            },
            new()
            {
                Id = 6,
                IdSede=1,
                Nombre ="Emergencias"
            },
        ];

        modelBuilder.Entity<Departamento>().HasData(departamentos);

        Usuario[] usuarios =
        [
            new()
            {
                Rut=21914307,
                Dv='9',
                Nombre ="Jugador 1",
                Genero = Generos.M,
                FechaNacimiento = new DateOnly(1982, 10, 24),
                Clave = "1234".Hash(),
                Piso=4,
                EsMonitor=false,
                IdDepartamento=4,
                Roles=AppRoles.Player
            },
            new()
            {
                Rut=20078534,
                Dv='7',
                Nombre ="Jugador 2",
                Genero = Generos.N,
                FechaNacimiento = new DateOnly(1985, 6, 6),
                Clave = "1234".Hash(),
                Piso=4,
                EsMonitor=true,
                IdDepartamento=4,
                Roles=AppRoles.Player
            },
            new()
            {
                Rut=19177149,
                Dv='4',
                Nombre ="Jugador 3",
                Genero = Generos.F,
                FechaNacimiento = new DateOnly(1990, 3, 14),
                Clave = "1234".Hash(),
                Piso=4,
                EsMonitor=false,
                IdDepartamento=4,
                Roles=AppRoles.Player
            },
            new()
            {
                Rut=15031942,
                Dv='0',
                Nombre ="Admin",
                Genero = Generos.M,
                FechaNacimiento = new DateOnly(1979, 7, 12),
                Clave = "1234".Hash(),
                Piso=5,
                EsMonitor=false,
                IdDepartamento=3,
                Roles=AppRoles.Admin
            },
            new()
            {
                Rut=16432549,
                Dv='0',
                Nombre ="SAdmin",
                Genero = Generos.M,
                FechaNacimiento = new DateOnly(1987, 3, 6),
                Clave = "1234".Hash(),
                Piso=5,
                EsMonitor=false,
                IdDepartamento=3,
                Roles=AppRoles.Admin | AppRoles.Player
            },
            new()
            {
                Rut=23063301,
                Dv='0',
                Nombre ="Fantasma",
                Genero = Generos.N,
                FechaNacimiento = new DateOnly(1980, 4, 18),
                Clave = "1234".Hash(),
                Piso=5,
                EsMonitor=false,
                IdDepartamento=3,
                Roles=AppRoles.None
            },

        ];

        modelBuilder.Entity<Usuario>().HasData(usuarios);

        Nivel[] niveles =
        [
            new()
            {
                Id = 1,
                Nombre = "Incendio Oficina Nivel 1",
                FuegosApagadosEsperados = 5,
                ExtintoresUsadosEsperados = 2,
                UsoAlarmaEsperado = true
            },
            new()
            {
                Id = 2,
                Nombre = "Fuego Cocina",
                FuegosApagadosEsperados = 3,
                ExtintoresUsadosEsperados = 1,
                UsoAlarmaEsperado = false
            }
        ];

        modelBuilder.Entity<Nivel>().HasData(niveles);
    }
}
