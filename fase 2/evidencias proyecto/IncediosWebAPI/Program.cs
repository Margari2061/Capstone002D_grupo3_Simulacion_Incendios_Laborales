
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


// ✅ CORREGIDO: Configuración simplificada de Autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth";  // ✅ Mayúscula
        options.AccessDeniedPath = "/Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ CORREGIDO: HSTS solo en PRODUCCIÓN
if (!app.Environment.IsDevelopment())  // ✅ Agregar "!"
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ CORREGIDO: Ruta default a AUTH (no a inicio)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}"  // ✅ Auth con mayúscula
);

app.Run();
