// DEFINE LOS ENDPOINTS PARA GESTIÓN DE USUARIOS
// LOS MÓDULOS NECESARIOS SON: USUARIO, PARTIDA Y ETL.

using IncediosWebAPI.Extensions;
using IncediosWebAPI.Model;
using IncediosWebAPI.Model.DataTransfer;
using IncediosWebAPI.Model.Domain;
using Microsoft.EntityFrameworkCore;

namespace IncediosWebAPI.Modules;

public static class UsuarioModule
{
    // ==================== EMPRESAS Y DEPARTAMENTOS ====================

    // GET /api/empresas - Obtener todas las empresas
    public static async Task<IResult> GetEmpresas(IncendioContext context)
    {
        try
        {
            var empresas = await context.Empresas.ToArrayAsync();
            return Results.Ok(empresas);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error al obtener empresas: {ex.Message}");
        }
    }

    // GET /api/sedes/{rutEmpresa} - Obtener sedes por empresa
    public static async Task<IResult> GetSedesPorEmpresa(int rutEmpresa, IncendioContext context)
    {
        try
        {
            var sedes = await context.Sedes
                .Where(s => s.RutEmpresa == rutEmpresa)
                .ToArrayAsync();

            return Results.Ok(sedes);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error al obtener sedes: {ex.Message}");
        }
    }

    // GET /api/departamentos/{idSede} - Obtener departamentos por sede
    public static async Task<IResult> GetDepartamentosPorSede(int idSede, IncendioContext context)
    {
        try
        {
            var departamentos = await context.Departamentos
                .Where(d => d.IdSede == idSede)
                .ToArrayAsync();

            return Results.Ok(departamentos);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error al obtener departamentos: {ex.Message}");
        }
    }

    // ==================== USUARIOS ====================

    // POST /api/usuarios/registro - Registrar nuevo usuario
    public static async Task<IResult> RegistrarUsuario(UsuarioRegistroDTO registroDto, IncendioContext context)
    {
        try
        {
            // Validar que el RUT no exista
            var usuarioExistente = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Rut == registroDto.Rut || u.Mail == registroDto.Mail);

            if (usuarioExistente != null)
            {
                return Results.Conflict("El RUT o email ya están registrados");
            }

            // Validar que el departamento exista
            var departamentoExistente = await context.Departamentos
                .FirstOrDefaultAsync(d => d.Id == registroDto.IdDepartamento);

            if (departamentoExistente == null)
            {
                return Results.BadRequest("El departamento especificado no existe");
            }

            // Crear nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Rut = registroDto.Rut,
                Dv = registroDto.Dv,
                Nombre = registroDto.Nombre,
                Mail = registroDto.Mail,
                Genero = registroDto.Genero,
                FechaNacimiento = registroDto.FechaNacimiento,
                Clave = registroDto.Password.Hash(), // Hash de contraseña
                Piso = registroDto.Piso,
                EsMonitor = registroDto.EsMonitor,
                IdDepartamento = registroDto.IdDepartamento
            };

            context.Usuarios.Add(nuevoUsuario);
            await context.SaveChangesAsync();

            // Retornar respuesta sin información sensible
            var response = new UsuarioResponseDTO
            {
                Rut = nuevoUsuario.Rut,
                Nombre = nuevoUsuario.Nombre,
                Mail = nuevoUsuario.Mail,
                Genero = nuevoUsuario.Genero,
                EsMonitor = nuevoUsuario.EsMonitor,
                IdDepartamento = nuevoUsuario.IdDepartamento
            };

            return Results.Created($"/api/usuarios/{nuevoUsuario.Rut}", new
            {
                message = "Usuario registrado exitosamente",
                usuario = response
            });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error al registrar usuario: {ex.Message}");
        }
    }

    // POST /api/usuarios/login - Autenticar usuario
    public static async Task<IResult> LoginUsuario(UsuarioLoginDTO loginDto, IncendioContext context)
    {
        try
        {
            // Buscar usuario por email
            var usuario = await context.Usuarios
                .Include(u => u.Departamento)
                .FirstOrDefaultAsync(u => u.Mail == loginDto.Mail);

            if (usuario == null || loginDto.Password.Hash() != usuario.Clave)
            {
                return Results.Unauthorized();
            }

            // Retornar información del usuario (sin datos sensibles)
            var response = new UsuarioResponseDTO
            {
                Rut = usuario.Rut,
                Nombre = usuario.Nombre,
                Mail = usuario.Mail,
                Genero = usuario.Genero,
                EsMonitor = usuario.EsMonitor,
                IdDepartamento = usuario.IdDepartamento
            };

            return Results.Ok(new
            {
                message = "Login exitoso",
                usuario = response
            });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error durante el login: {ex.Message}");
        }
    }

    // GET /api/usuarios/{rut} - Obtener usuario por RUT
    public static async Task<IResult> GetUsuarioPorRut(int rut, IncendioContext context)
    {
        try
        {
            var usuario = await context.Usuarios
                .Include(u => u.Departamento)
                .FirstOrDefaultAsync(u => u.Rut == rut);

            if (usuario == null)
            {
                return Results.NotFound("Usuario no encontrado");
            }

            var response = new UsuarioResponseDTO
            {
                Rut = usuario.Rut,
                Nombre = usuario.Nombre,
                Mail = usuario.Mail,
                Genero = usuario.Genero,
                EsMonitor = usuario.EsMonitor,
                IdDepartamento = usuario.IdDepartamento
            };

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error al obtener usuario: {ex.Message}");
        }
    }

    // GET /api/usuarios/{rut}/partidas - Obtener partidas de un usuario
    public static async Task<IResult> GetPartidasPorUsuario(int rut, IncendioContext context)
    {
        try
        {
            var partidas = await context.Partidas
                .Include(p => p.NivelNavigation)
                .Where(p => p.RutUsuario == rut)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            return Results.Ok(partidas);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error al obtener partidas: {ex.Message}");
        }
    }
}