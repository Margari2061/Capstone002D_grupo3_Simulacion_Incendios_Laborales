
// CONFIGURAR Y EJECUTAR LA APLICACI�N 

using IncediosWebAPI.Model;
using IncediosWebAPI.Model.Domain;
using IncediosWebAPI.Modules;
using Microsoft.EntityFrameworkCore;
using IncediosWebAPI.Model.DataTransfer;

var builder = WebApplication.CreateBuilder(args);


// Configuraci�n de la base de datos 
string connectionString = "Data Source=localhost;Initial Catalog=operacional;User Id=joan;Password=joan;Integrated Security=False;Connect Timeout=3600;Encrypt=False;TrustServerCertificate=True";
builder.Services.AddDbContextPool<IncendioContext>(options => options.UseSqlServer(connectionString));

//----------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configurar Swagger en desarrollo

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(op =>
    {
        op.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        op.RoutePrefix = "";
    });
}




// ==================== USUARIO - LOGIN ====================
app.MapPost("/login", async (UsuarioLoginDTO loginDto, IncendioContext context) =>
    await UsuarioModule.LoginUsuario(loginDto, context));


// ==================== USUARIO - PERFIL ====================
app.MapGet("/usuarios/{rut}", async (int rut, IncendioContext context) =>
    await UsuarioModule.GetUsuarioPorRut(rut, context));





// -----------------------------------------------------------------------------------------------------------------
// ==================== PARTIDA - COMENZAR_PARTIDA ====================
app.MapPost("/partidas", async (PartidaCreateDTO partidaDto, IncendioContext context) =>
    await PartidaModule.CrearPartida(partidaDto, context));

app.MapGet("/usuarios/{rut}/partidas", async (int rut, IncendioContext context) =>
    await PartidaModule.GetPartidasPorUsuario(rut, context));

app.MapGet("/partidas/estadisticas", async (IncendioContext context) =>
    await PartidaModule.GetEstadisticasGenerales(context));


// ==================== PARTIDA - FINALIZAR ====================  
app.MapPost("/partidas/finalizar", async (PartidaFinalizarDTO finalizarDto, IncendioContext context) =>
    await PartidaModule.FinalizarPartida(finalizarDto, context));

// -----------------------------------------------------------------------------------------------------------------


// ==================== ETL - COMENZAR_PROCESO ====================
// (Esto ir� en EtlModule.cs cuando lo crees)
app.MapPost("/etl/procesar", async (IncendioContext context) =>
{
    // L�gica temporal para proceso ETL
    try
    {
        //Transferir datos de Partidas a MetricasEvento
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
        .CountAsync(p => p.Resultado == '0'); // Condiciones Cumplidas

    return Results.Ok(new
    {
        totalUsuarios,
        totalPartidas,
        partidasExitosas,
        tasaExito = totalPartidas > 0 ? (double)partidasExitosas / totalPartidas : 0
    });
});

app.Run();
