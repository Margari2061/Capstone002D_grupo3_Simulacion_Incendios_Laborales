using IncediosWebAPI.Extensions;
using IncediosWebAPI.Model;
using IncediosWebAPI.Model.DataTransfer;
using IncediosWebAPI.Model.IncendioDB;
using IncediosWebAPI.Model.IncendioDB.Domain;
using IncediosWebAPI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IncediosWebAPI.Controllers;

[WebAuthorize(AppRoles.Admin)]
public class UsuariosController : Controller
{
    private readonly IncendioContext _context;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IncendioContext context, ILogger<UsuariosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        try
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Departamento)
                .ThenInclude(d => d.Sede)
                .ThenInclude(s => s.Empresa)
                .ToListAsync();

            return View(usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar lista de usuarios");
            TempData["Error"] = "Error al cargar los usuarios";
            return View(new List<Usuario>());
        }
    }

    // GET: Usuarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            TempData["Error"] = "ID de usuario no especificado";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            UsuarioDetails model = new();

            var usuario = await _context.Usuarios
                .Include(u => u.Departamento)
                .Include(u => u.Partidas)
                .ThenInclude(p => p.Nivel)
                .FirstOrDefaultAsync(m => m.Rut == id);

            if (usuario == null)
            {
                TempData["Error"] = $"Usuario con RUT {id} no encontrado";
                return RedirectToAction(nameof(Index));
            }

            model.User = usuario;

            TimeSpan played = TimeSpan.Zero;
            Partida[] partidas = usuario
                .Partidas
                .Where(p => p.Resultado != ResultadosPartida.EnProgreso)
                .ToArray();

            foreach (Partida run in partidas)
            {
                played += run.TiempoJugado;
            }

            double totalDamage = partidas.Sum(p => p.Heridas);
            int totalRuns = partidas.Length;
            double totalExtinguished = partidas.Sum(p => p.FuegosApagados);
            double totalExtinguisherUsed = partidas.Sum(p => p.ExtintoresUsados);

            double dmgRun = Math.Round(totalDamage / totalRuns, 2);
            if (double.IsInfinity(dmgRun) || double.IsNaN(dmgRun))
                dmgRun = 0;

            double avgExt = Math.Round(totalExtinguisherUsed / totalExtinguished, 2);
            if(double.IsInfinity(avgExt) || double.IsNaN(avgExt))
                avgExt = 1;

            model.TimePlayed = played.ToString(@"hh\:mm\:ss");
            model.PlayedRuns = totalRuns;
            model.DamagePerRuns = dmgRun;
            model.AverageExtintionRatio = avgExt; 

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar detalles del usuario {UsuarioId}", id);
            TempData["Error"] = "Error al cargar los detalles del usuario";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Usuarios/Create
    public IActionResult Create()
    {
        try
        {
            LoadViewData();
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar vista de creación de usuario");
            TempData["Error"] = "Error al cargar el formulario de creación";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioRegistroDTO model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                AddModelErrors("Por favor, corrija los errores del formulario");
                LoadViewData();
                return View(model);
            }

            // Validar y parsear RUT
            if (!TryParseRut(model.Rut, out int rut, out char dv))
            {
                AddModelErrors("Formato de RUT inválido. Use: 12345678-9");
                LoadViewData();
                return View(model);
            }

            // Verificar que el RUT no exista
            if (await _context.Usuarios.AnyAsync(u => u.Rut == rut))
            {
                AddModelErrors($"El RUT {model.Rut} ya está registrado");
                LoadViewData();
                return View(model);
            }

            // Crear roles
            AppRoles roles = AppRoles.None;
            if (model.EsPlayer) roles |= AppRoles.Player;
            if (model.EsAdmin) roles |= AppRoles.Admin;

            // Validar que tenga al menos un rol
            if (roles == AppRoles.None)
            {
                AddModelErrors("El usuario debe tener al menos un rol (Player o Admin)");
                LoadViewData();
                return View(model);
            }

            var usuario = new Usuario
            {
                Rut = rut,
                Dv = dv,
                Nombre = model.Nombre.Trim(),
                Genero = model.Genero,
                FechaNacimiento = model.FechaNacimiento,
                Clave = model.Clave.Hash(),
                Piso = model.Piso,
                EsMonitor = model.EsMonitor,
                IdDepartamento = model.IdDepartamento,
                Roles = roles
            };

            _context.Add(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario creado exitosamente: {Nombre} ({Rut})", usuario.Nombre, usuario.Rut);
            TempData["Success"] = $"Usuario {usuario.Nombre} creado exitosamente";

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos al crear usuario");
            AddModelErrors("Error al guardar en la base de datos. Verifique los datos.");
            LoadViewData();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear usuario");
            AddModelErrors("Error interno del sistema. Intente nuevamente.");
            LoadViewData();
            return View(model);
        }
    }

    // GET: Usuarios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            TempData["Error"] = "ID de usuario no especificado";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                TempData["Error"] = $"Usuario con RUT {id} no encontrado";
                return RedirectToAction(nameof(Index));
            }

            var dto = new UsuarioRegistroDTO
            {
                Rut = $"{usuario.Rut}-{usuario.Dv}",
                Nombre = usuario.Nombre,
                Genero = usuario.Genero,
                FechaNacimiento = usuario.FechaNacimiento,
                Clave = "", // No mostrar contraseña actual
                Piso = usuario.Piso,
                EsMonitor = usuario.EsMonitor,
                IdDepartamento = usuario.IdDepartamento,
                EsPlayer = usuario.Roles.HasFlag(AppRoles.Player),
                EsAdmin = usuario.Roles.HasFlag(AppRoles.Admin),
            };

            LoadViewData(usuario.Genero, usuario.IdDepartamento);
            return View(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar edición del usuario {UsuarioId}", id);
            TempData["Error"] = "Error al cargar el formulario de edición";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Usuarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioRegistroDTO model)
    {
        try
        {
            if (!TryParseRut(model.Rut, out int rut, out char dv))
            {
                AddModelErrors("Formato de RUT inválido");
                LoadViewData(model.Genero, model.IdDepartamento);
                return View(model);
            }

            if (id != rut)
            {
                TempData["Error"] = "El RUT no puede ser modificado";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("Clave"); // La contraseña es opcional en edición

            if (!ModelState.IsValid)
            {
                AddModelErrors("Por favor, corrija los errores del formulario");
                LoadViewData(model.Genero, model.IdDepartamento);
                return View(model);
            }

            var usuario = await _context.Usuarios.FirstAsync(u => u.Rut == rut);

            // Crear roles
            AppRoles roles = AppRoles.None;
            if (model.EsPlayer) roles |= AppRoles.Player;
            if (model.EsAdmin) roles |= AppRoles.Admin;

            if (roles == AppRoles.None)
            {
                AddModelErrors("El usuario debe tener al menos un rol");
                LoadViewData(model.Genero, model.IdDepartamento);
                return View(model);
            }

            // Actualizar datos
            usuario.Nombre = model.Nombre.Trim();
            usuario.Genero = model.Genero;
            usuario.FechaNacimiento = model.FechaNacimiento;
            usuario.Piso = model.Piso;
            usuario.EsMonitor = model.EsMonitor;
            usuario.IdDepartamento = model.IdDepartamento;
            usuario.Roles = roles;

            // Actualizar contraseña solo si se proporcionó una nueva
            if (!string.IsNullOrWhiteSpace(model.Clave))
            {
                usuario.Clave = model.Clave.Hash();
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario actualizado exitosamente: {Nombre} ({Rut})", usuario.Nombre, usuario.Rut);
            TempData["Success"] = $"Usuario {usuario.Nombre} actualizado exitosamente";

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UsuarioExists(id))
            {
                TempData["Error"] = "El usuario ya no existe";
                return RedirectToAction(nameof(Index));
            }
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {UsuarioId}", id);
            AddModelErrors("Error al actualizar el usuario");
            LoadViewData(model.Genero, model.IdDepartamento);
            return View(model);
        }
    }

    // GET: Usuarios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            TempData["Error"] = "ID de usuario no especificado";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Departamento)
                .Include(u => u.Partidas)
                .FirstOrDefaultAsync(m => m.Rut == id);

            if (usuario == null)
            {
                TempData["Error"] = $"Usuario con RUT {id} no encontrado";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si tiene partidas asociadas
            if (usuario.Partidas.Any())
            {
                TempData["Warning"] = $"El usuario tiene {usuario.Partidas.Count} partidas asociadas. Estas también serán eliminadas.";
            }

            return View(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar eliminación del usuario {UsuarioId}", id);
            TempData["Error"] = "Error al cargar la confirmación de eliminación";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Usuarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Partidas)
                .FirstOrDefaultAsync(u => u.Rut == id);

            if (usuario == null)
            {
                TempData["Error"] = $"Usuario con RUT {id} no encontrado";
                return RedirectToAction(nameof(Index));
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario eliminado: {Nombre} ({Rut})", usuario.Nombre, usuario.Rut);
            TempData["Success"] = $"Usuario {usuario.Nombre} eliminado exitosamente";

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos al eliminar usuario {UsuarioId}", id);
            TempData["Error"] = "Error al eliminar el usuario. Puede tener datos asociados.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar usuario {UsuarioId}", id);
            TempData["Error"] = "Error interno al eliminar el usuario";
            return RedirectToAction(nameof(Index));
        }
    }

    private bool UsuarioExists(int id)
    {
        return _context.Usuarios.Any(e => e.Rut == id);
    }

    // ==================== MÉTODOS AUXILIARES ====================

    private bool TryParseRut(string rutStr, out int rut, out char dv)
    {
        rut = 0;
        dv = '0';

        try
        {
            var cleanedRut = rutStr.Replace(".", "").ToUpper();
            var parts = cleanedRut.Split('-');

            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[0], out rut)) return false;
            if (!char.TryParse(parts[1], out dv)) return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void LoadViewData(Generos? selectedGenero = null, int? selectedDepartamento = null)
    {
        var generos = new Dictionary<Generos, string>
        {
            { Generos.M, "Masculino" },
            { Generos.F, "Femenino" },
            { Generos.N, "No informa" },
        };

        ViewData["Genero"] = new SelectList(generos, "Key", "Value", selectedGenero);
        ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "Id", "Nombre", selectedDepartamento);
    }

    private void AddModelErrors(string message)
    {
        ModelState.AddModelError("", message);
        TempData["Error"] = message;
    }
}