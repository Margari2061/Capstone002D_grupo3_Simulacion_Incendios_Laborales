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
using System.Security.Claims;

namespace IncediosWebAPI.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly IncendioContext _context;

    public AuthController(IncendioContext context)
    {
        _context = context;
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
                new Claim("login", teils[0]),
            };

            // Agregar roles
            foreach (AppRoles role in Enum.GetValues(typeof(AppRoles)))
            {
                if (role != AppRoles.None && user.Roles.HasFlag(role))
                {
                    claims.Add(new Claim("role", role.ToString()));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, "Basic");

            await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Inicio");
        }
        catch (Exception ex)
        {
            AddModelError("Error interno del sistema");
            return View("Index", model);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("index", "auth");
    }

    // ... (método ApiLogin se mantiene igual) ...

    private void AddModelError(string message)
    {
        ModelState.AddModelError("", message);
        TempData["Error"] = message;
    }
}