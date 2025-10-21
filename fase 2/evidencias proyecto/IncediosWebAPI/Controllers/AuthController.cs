using IncediosWebAPI.Extensions;
using IncediosWebAPI.Model;
using IncediosWebAPI.Model.DataTransfer;
using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model.IncendioDB.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IncediosWebAPI.Controllers;

public class AuthController : Controller
{
    private readonly IncendioContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IncendioContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UsuarioLoginDTO model)
    {
        if (!ModelState.IsValid)
        {
            AddModelError("Por favor, complete todos los campos correctamente");
            return View("Index", model);
        }

        try
        {
            // Limpiar y validar formato RUT
            model.Rut = model.Rut.Replace(".", "").ToUpper();
            string[] teils = model.Rut.Split('-');

            if (teils.Length != 2)
            {
                AddModelError("Formato de RUT inválido. Use: 12345678-9");
                return View("Index", model);
            }

            if (!int.TryParse(teils[0], out int rut))
            {
                AddModelError("El RUT debe contener solo números antes del guión");
                return View("Index", model);
            }

            if (!char.TryParse(teils[1], out char dv))
            {
                AddModelError("El dígito verificador debe ser un carácter");
                return View("Index", model);
            }

            // Buscar usuario
            Usuario? user = await _context.Usuarios
                .Where(u => u.Rut == rut && u.Dv == dv)
                .FirstOrDefaultAsync();

            if (user == null || user.Clave != model.Password.Hash())
            {
                _logger.LogWarning("Intento de login fallido para RUT: {Rut}", rut);
                AddModelError("RUT o contraseña incorrectos");
                return View("Index", model);
            }

            if (user.Roles == AppRoles.None)
            {
                AddModelError("Usuario no tiene permisos para acceder al sistema");
                return View("Index", model);
            }

            // Crear claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Rut.ToString()),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim("Rut", user.Rut.ToString()),
                new Claim("Dv", user.Dv.ToString())
            };

            // Agregar roles
            foreach (AppRoles role in Enum.GetValues(typeof(AppRoles)))
            {
                if (role != AppRoles.None && user.Roles.HasFlag(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("Login exitoso: {Nombre} ({Rut})", user.Nombre, rut);

            return RedirectToAction("Index", "Inicio");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login para RUT: {Rut}", model.Rut);
            AddModelError("Error interno del sistema");
            return View("Index", model);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Logout exitoso");
        return RedirectToAction("Index");
    }

    // ... (método ApiLogin se mantiene igual) ...

    private void AddModelError(string message)
    {
        ModelState.AddModelError("", message);
        TempData["Error"] = message;
    }
}