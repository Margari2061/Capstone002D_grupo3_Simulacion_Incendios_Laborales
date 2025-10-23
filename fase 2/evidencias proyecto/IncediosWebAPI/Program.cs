
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model;

var builder = WebApplication.CreateBuilder(args);


// Configuracion de la base de datos 
IConfigurationSection dbSection = builder.Configuration.GetSection("DB");
builder.Services.AddDbContextPool<IncendioContext>(options => options.UseSqlServer(dbSection.GetValue<string>("Operacional")));


IConfigurationSection appSettingsSection = builder.Configuration.GetSection("AppSettings");
AppSettings.Initialize(appSettingsSection);


// Configuración simplificada de Autenticación
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/auth";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// CORREGIDO: HSTS solo en PRODUCCIÓN
if (!app.Environment.IsDevelopment())  // Agregar "!"
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=inicio}/{action=index}/{id?}" 
);

app.Run();
