using Humanizer;
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

    public UsuariosController(IncendioContext context)
    {
        _context = context;
    }

    // GET: Usuarios
    public async Task<IActionResult> Index()
    {
        var incendioContext = _context.Usuarios.Include(u => u.Departamento);
        return View(await incendioContext.ToListAsync());
    }

    // GET: Usuarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Departamento)
            .FirstOrDefaultAsync(m => m.Rut == id);
        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    // GET: Usuarios/Create
    public IActionResult Create()
    {
        Dictionary<Generos, string> genres = new()
        {
            { Generos.M, "Masculino"},
            { Generos.F, "Femenino"},
            { Generos.N, "No informa"},
        };

        ViewData["Genero"] = new SelectList(genres, "Key", "Value");
        ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "Id", "Nombre");
        return View();
    }

    // POST: Usuarios/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Rut,Dv,Nombre,Genero,FechaNacimiento,Clave,Piso,EsMonitor,IdDepartamento,EsPlayer,EsAdmin")] UsuarioRegistroDTO model)
    {
        if (ModelState.IsValid)
        {
            AppRoles roles = AppRoles.None;
            if (model.EsPlayer)
                roles |= AppRoles.Player;

            if (model.EsAdmin)
                roles |= AppRoles.Admin;

            Usuario user = new()
            {
                Rut = model.Rut,
                Dv = model.Dv,
                Nombre = model.Nombre,
                Genero = model.Genero,
                FechaNacimiento = model.FechaNacimiento,
                Clave = model.Clave.Hash(),
                Piso = model.Piso,
                EsMonitor = model.EsMonitor,
                IdDepartamento = model.IdDepartamento,
                Roles = roles
            };

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        Dictionary<Generos, string> genres = new()
        {
            { Generos.M, "Masculino"},
            { Generos.F, "Femenino"},
            { Generos.N, "No informa"},
        };

        ViewData["Genero"] = new SelectList(genres, "Key", "Value", model.Genero);
        ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "Id", "Nombre", model.IdDepartamento);
        return View(model);
    }

    // GET: Usuarios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        UsuarioRegistroDTO dto = new()
        {
            Rut=usuario.Rut,
            Dv=usuario.Dv,
            Nombre=usuario.Nombre,
            Genero=usuario.Genero,
            FechaNacimiento=usuario.FechaNacimiento,
            Clave="",
            Piso=usuario.Piso,
            EsMonitor=usuario.EsMonitor,
            IdDepartamento=usuario.IdDepartamento,
            EsPlayer=usuario.Roles.HasFlag(AppRoles.Player),
            EsAdmin=usuario.Roles.HasFlag(AppRoles.Admin),
        };

        Dictionary<Generos, string> genres = new()
        {
            { Generos.M, "Masculino"},
            { Generos.F, "Femenino"},
            { Generos.N, "No informa"},
        };

        ViewData["Genero"] = new SelectList(genres, "Key", "Value", dto.Genero);
        ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "Id", "Nombre", usuario.IdDepartamento);
        return View(dto);
    }

    // POST: Usuarios/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Rut,Dv,Nombre,Genero,FechaNacimiento,Clave,Piso,EsMonitor,IdDepartamento,EsPlayer,EsAdmin")] UsuarioRegistroDTO model)
    {
        if (id != model.Rut)
        {
            return NotFound();
        }

        ModelState.Remove("Clave");

        if (ModelState.IsValid)
        {
            try
            {
                Usuario user = await _context
                    .Usuarios
                    .FirstAsync(u => u.Rut == model.Rut);

                AppRoles roles = AppRoles.None;
                if (model.EsPlayer)
                    roles |= AppRoles.Player;

                if (model.EsAdmin)
                    roles |= AppRoles.Admin;

                user.Nombre = model.Nombre;
                user.Genero = model.Genero;
                user.FechaNacimiento = model.FechaNacimiento;
                user.Piso = model.Piso;
                user.EsMonitor = model.EsMonitor;
                user.IdDepartamento = model.IdDepartamento;
                user.Roles = roles;

                if(!string.IsNullOrWhiteSpace(model.Clave))
                    user.Clave = model.Clave.Hash();
                
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(model.Rut))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        Dictionary<Generos, string> genres = new()
        {
            { Generos.M, "Masculino"},
            { Generos.F, "Femenino"},
            { Generos.N, "No informa"},
        };

        ViewData["Genero"] = new SelectList(genres, "Key", "Value", model.Genero);
        ViewData["IdDepartamento"] = new SelectList(_context.Departamentos, "Id", "Nombre", model.IdDepartamento);
        return View(model);
    }

    // GET: Usuarios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Departamento)
            .FirstOrDefaultAsync(m => m.Rut == id);
        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    // POST: Usuarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UsuarioExists(int id)
    {
        return _context.Usuarios.Any(e => e.Rut == id);
    }
}
