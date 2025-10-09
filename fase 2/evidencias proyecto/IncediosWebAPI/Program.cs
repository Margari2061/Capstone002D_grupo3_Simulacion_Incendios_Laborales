
// CONFIGURAR Y EJECUTAR LA APLICACI�N 

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using IncediosWebAPI.Model.IncendioDB;

var builder = WebApplication.CreateBuilder(args);


// Configuracion de la base de datos 
IConfigurationSection dbSection = builder.Configuration.GetSection("DB");
builder.Services.AddDbContextPool<IncendioContext>(options => options.UseSqlServer(dbSection.GetValue<string>("Operacional")));

// Configurar DW -- TODO

// Configurar Autenticacion por Cookie
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/index";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseHsts();    
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute
(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}"
);

//// ==================== DASHBOARD ====================
//app.MapGet("/dashboard/estadisticas", async (IncendioContext context) =>
//{
//    // Estad�sticas b�sicas para el dashboard
//    var totalUsuarios = await context.Usuarios.CountAsync();
//    var totalPartidas = await context.Partidas.CountAsync();
//    var partidasExitosas = await context.Partidas
//        .CountAsync(p => p.Resultado == '0'); // Condiciones Cumplidas

//    return Results.Ok(new
//    {
//        totalUsuarios,
//        totalPartidas,
//        partidasExitosas,
//        tasaExito = totalPartidas > 0 ? (double)partidasExitosas / totalPartidas : 0
//    });
//});

app.Run();
