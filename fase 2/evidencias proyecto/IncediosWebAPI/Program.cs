
// CONFIGURAR Y EJECUTAR LA APLICACI�N 

using IncediosWebAPI.Model.IncendioDB.Domain;
using IncediosWebAPI.Modules;
using Microsoft.EntityFrameworkCore;
using IncediosWebAPI.Model.DataTransfer;
using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model;

var builder = WebApplication.CreateBuilder(args);


// Configuraci�n de la base de datos 
string connectionString = "Data Source=localhost;Initial Catalog=operacional;User Id=joan;Password=joan;Integrated Security=False;Connect Timeout=3600;Encrypt=False;TrustServerCertificate=True";
builder.Services.AddDbContextPool<IncendioContext>(options => options.UseSqlServer(connectionString));



//----------------------------------


var app = builder.Build();


// Configurar Swagger en desarrollo

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//------------------------------ Lee las empresas en la base de datos ---------------------------------

app.MapGet("/empresas", async (IncendioContext context) => await UsuarioModule.GetEmpresas(context));

app.MapPost("/empresas", async (
    Empresa req,
    IncendioContext context
    ) =>
{

    return "Ok";
});

// ==================== SEDES ====================
app.MapGet("/sedes/{rutEmpresa}", async (int rutEmpresa, IncendioContext context) =>
    await UsuarioModule.GetSedesPorEmpresa(rutEmpresa, context));

// ==================== DEPARTAMENTOS ====================
app.MapGet("/departamentos/{idSede}", async (int idSede, IncendioContext context) =>
    await UsuarioModule.GetDepartamentosPorSede(idSede, context));

// ==================== USUARIO - LOGIN ====================
app.MapPost("/usuarios/login", async (UsuarioLoginDTO loginDto, IncendioContext context) =>
    await UsuarioModule.LoginUsuario(loginDto, context));

// ==================== USUARIO - PERFIL ====================
app.MapGet("/usuarios/{rut}", async (int rut, IncendioContext context) =>
    await UsuarioModule.GetUsuarioPorRut(rut, context));

// ==================== PARTIDA - HISTORIAL ====================
app.MapGet("/usuarios/{rut}/partidas", async (int rut, IncendioContext context) =>
    await UsuarioModule.GetPartidasPorUsuario(rut, context));

// ==================== PARTIDA - COMENZAR_PARTIDA ====================
// (Esto ir� en PartidaModule.cs cuando lo crees)
app.MapPost("/partidas", async (PartidaCreateDTO partidaDto, IncendioContext context) 
    => await PartidaModule.CrearPartida(partidaDto, context));

// ==================== ETL - COMENZAR_PROCESO ====================
// (Esto ir� en EtlModule.cs cuando lo crees)
app.MapPost("/etl/procesar", async (IncendioContext context) =>
{
    // L�gica temporal para proceso ETL
    try
    {
        // Ejemplo: Transferir datos de Partidas a MetricasEvento
        var partidasRecientes = await context.Partidas
            .Where(p => p.Fecha >= DateTime.UtcNow.AddDays(-1))
            .ToListAsync();

        // Aqu� ir�a la l�gica de transformaci�n y carga al Data Warehouse
        return Results.Ok(new
        {
            message = "Proceso ETL iniciado",
            partidasProcesadas = partidasRecientes.Count
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error en proceso ETL: {ex.Message}");
    }
});

// ==================== DASHBOARD ====================
app.MapGet("/dashboard/estadisticas", async (IncendioContext context) =>
{
    // Estad�sticas b�sicas para el dashboard
    var totalUsuarios = await context.Usuarios.CountAsync();
    var totalPartidas = await context.Partidas.CountAsync();
    var partidasExitosas = await context.Partidas
        .CountAsync(p => p.Resultado == ResultadosPartida.CondicionesCumplidas); // Condiciones Cumplidas

    return Results.Ok(new
    {
        totalUsuarios,
        totalPartidas,
        partidasExitosas,
        tasaExito = totalPartidas > 0 ? (double)partidasExitosas / totalPartidas : 0
    });
});

app.Run();
