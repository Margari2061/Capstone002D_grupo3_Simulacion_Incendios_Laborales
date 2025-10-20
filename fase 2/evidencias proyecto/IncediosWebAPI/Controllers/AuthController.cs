using IncediosWebAPI.Extensions;
using IncediosWebAPI.Model;
using IncediosWebAPI.Model.DataTransfer;
using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model.IncendioDB.Domain;
using Microsoft.AspNetCore.Authentication;
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
            return RedirectToAction(nameof(Index));
        }

        try
        {
            // Limpiar y validar formato RUT
            model.Rut = model.Rut.Replace(".", "").ToUpper();

            string[] teils = model.Rut.Split('-');

            if (teils.Length != 2)
            {
                AddModelError("Formato de RUT inválido. Use: 12345678-9");
                return RedirectToAction(nameof(Index));
            }

            if (!int.TryParse(teils[0], out int rut))
            {
                AddModelError("El RUT debe contener solo números antes del guión");
                return RedirectToAction(nameof(Index));
            }

            if (!char.TryParse(teils[1], out char dv))
            {
                AddModelError("El dígito verificador debe ser un carácter");
                return RedirectToAction(nameof(Index));
            }

            // Buscar usuario
            Usuario? user = await _context.Usuarios
                .Where(u => u.Rut == rut && u.Dv == dv)
                .FirstOrDefaultAsync();

            if (user == null || user.Clave != model.Password.Hash())
            {
                // Log para debugging (sin revelar información sensible)
                _logger.LogWarning("Intento de login fallido para RUT: {Rut}", rut);
                AddModelError("RUT o contraseña incorrectos");
                return RedirectToAction(nameof(Index));
            }

            // Verificar que el usuario tenga al menos un rol activo
            if (user.Roles == AppRoles.None)
            {
                AddModelError("Usuario no tiene permisos para acceder al sistema");
                return RedirectToAction(nameof(Index));
            }

            // Crear claims
            List<Claim> claims =
            [
                new Claim("login", $"{rut}-{dv}"),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim("rut", rut.ToString()),
            ];

            // Agregar roles como claims
            foreach (AppRoles role in Enum.GetValues(typeof(AppRoles)))
            {
                if (role == AppRoles.None)
                    continue;

                if (user.Roles.HasFlag(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                    claims.Add(new Claim("role", role.ToString()));
                }
            }

            ClaimsIdentity identity = new(claims, "Basic");
            ClaimsPrincipal principal = new(identity);

            await HttpContext.SignInAsync(principal);

            _logger.LogInformation("Login exitoso para usuario: {Nombre} ({Rut})", user.Nombre, rut);

            return RedirectToAction("Index", "Inicio");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login para RUT: {Rut}", model.Rut);
            AddModelError("Error interno del sistema. Por favor, intente nuevamente.");
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Logout()
    {
        var userName = User.Identity?.Name;
        await HttpContext.SignOutAsync();
        _logger.LogInformation("Logout exitoso para usuario: {UserName}", userName);
        return RedirectToAction("Index");
    }

    [HttpPost, Route("api/auth")]
    public async Task<IActionResult> ApiLogin([FromBody] UsuarioLoginDTO model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Datos de login inválidos" });

            // Limpiar y validar formato RUT
            model.Rut = model.Rut.Replace(".", "").ToUpper();
            string[] teils = model.Rut.Split('-');

            if (teils.Length != 2)
                return BadRequest(new { error = "Formato de RUT inválido. Use: 12345678-9" });

            if (!int.TryParse(teils[0], out int rut))
                return BadRequest(new { error = "RUT inválido" });

            if (!char.TryParse(teils[1], out char dv))
                return BadRequest(new { error = "Dígito verificador inválido" });

            // Buscar usuario
            Usuario? user = await _context.Usuarios
                .Where(u => u.Rut == rut && u.Dv == dv)
                .FirstOrDefaultAsync();

            if (user == null || user.Clave != model.Password.Hash())
            {
                _logger.LogWarning("API Login fallido para RUT: {Rut}", rut);
                return Unauthorized(new { error = "Credenciales inválidas" });
            }

            if (user.Roles == AppRoles.None)
                return Unauthorized(new { error = "Usuario sin permisos" });

            // Generar JWT
            string? key = AppSettings.Instance.JWT;

            if (string.IsNullOrEmpty(key))
            {
                _logger.LogError("JWT key no configurada");
                return StatusCode(500, new { error = "Error de configuración del servidor" });
            }

            JwtSecurityTokenHandler handler = new();
            byte[] keyBuffer = Convert.FromBase64String(key);
            SymmetricSecurityKey symKey = new(keyBuffer);

            DateTime expireDate = DateTime.UtcNow.AddDays(1);

            // Crear claims para JWT
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Rut.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Nombre),
                new Claim("rut", user.Rut.ToString()),
                new Claim("dv", user.Dv.ToString())
            };

            // Agregar roles
            foreach (AppRoles role in Enum.GetValues(typeof(AppRoles)))
            {
                if (role != AppRoles.None && user.Roles.HasFlag(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
            }

            SecurityTokenDescriptor descriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expireDate,
                SigningCredentials = new SigningCredentials(symKey, SecurityAlgorithms.HmacSha256),
                Issuer = "IncediosWebAPI",
                Audience = "IncediosClient"
            };

            SecurityToken token = handler.CreateToken(descriptor);
            string tokenString = handler.WriteToken(token);

            _logger.LogInformation("API Login exitoso para usuario: {Nombre}", user.Nombre);

            return Ok(new
            {
                token = tokenString,
                expires = expireDate,
                user = new
                {
                    rut = user.Rut,
                    nombre = user.Nombre,
                    roles = user.Roles.ToString()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en API Login para RUT: {Rut}", model.Rut);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    // ==================== MÉTODO AUXILIAR MEJORADO ====================
    private void AddModelError(string message)
    {
        ModelState.AddModelError("", message);
        TempData["Error"] = message;
        _logger.LogWarning("Error de validación en login: {Message}", message);
    }
}