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
            return RedirectToAction(nameof(Index));

        string[] teils = model
            .Rut
            .Split('-');

        if(teils.Length != 2)
            return RedirectToAction(nameof(Index));

        if(!int.TryParse(teils[0], out int rut))
            return RedirectToAction(nameof(Index));

        if (!char.TryParse(teils[1], out char dv))
            return RedirectToAction(nameof(Index));

        Usuario? user = await _context
            .Usuarios
            .Where(u => u.Rut == rut && u.Dv == dv)
            .FirstOrDefaultAsync();

        if(user == null)
            return RedirectToAction(nameof(Index));

        if(user.Clave != model.Password.Hash())
            return RedirectToAction(nameof(Index));

        List<Claim> claims =
        [
            new("login", teils[0]),
        ];

        foreach (AppRoles role in Enum.GetValues(typeof(AppRoles)))
        {
            if (role == AppRoles.None)
                continue;

            if (user.Roles.HasFlag(role))
                claims.Add(new("role", role.ToString()));
        }

        ClaimsIdentity identity = new(claims, "Basic");
        ClaimsPrincipal principal = new(identity);

        await HttpContext.SignInAsync(principal);

        return RedirectToAction("index", "inicio");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index");
    }

    [HttpPost, Route("api/auth")]
    public async Task<IActionResult> ApiLogin(UsuarioLoginDTO model)
    {
        string[] teils = model
            .Rut
            .Split('-');

        if (teils.Length != 2)
            return RedirectToAction(nameof(Index));

        if (!int.TryParse(teils[0], out int rut))
            return RedirectToAction(nameof(Index));

        if (!char.TryParse(teils[1], out char dv))
            return RedirectToAction(nameof(Index));

        Usuario? user = await _context
            .Usuarios
            .Where(u => u.Rut == rut && u.Dv == dv)
            .FirstOrDefaultAsync();

        if (user == null)
            return Unauthorized();

        if (user.Clave != model.Password.Hash())
            return Unauthorized();

        string? key = AppSettings.Instance.JWT;

        if (key is null)
            return BadRequest();

        JwtSecurityTokenHandler handler = new();

        byte[] keyBuffer = Convert.FromBase64String(key);
        SymmetricSecurityKey symKey = new(keyBuffer);

        DateTime expireDate = DateTime.UtcNow.AddDays(1);

        SecurityTokenDescriptor descriptor = new()
        {
            Expires = expireDate,
            SigningCredentials = new(symKey, SecurityAlgorithms.HmacSha256),
        };

        SecurityToken token = handler.CreateToken(descriptor);
        string tokenString = handler.WriteToken(token);

        return Ok(new 
        { 
            Key = tokenString, 
            Expires = expireDate 
        });
    }
}
