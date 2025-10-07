// DEFINE LOS ENDPOINTS PARA GESTIÓN DE USUARIOS
// LOS MÓDULOS NECESARIOS SON: USUARIO, PARTIDA Y ETL.

using IncediosWebAPI.Extensions;
using IncediosWebAPI.Model;
using IncediosWebAPI.Model.DataTransfer;
using IncediosWebAPI.Model.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IncediosWebAPI.Modules;

public static class UsuarioModule
{


    // ==================== USUARIOS ====================

    // POST /api/usuarios/login - Autenticar usuario
    public static async Task<IResult> LoginUsuario(UsuarioLoginDTO loginDto, IncendioContext context)
    {
        try
        {
            // Validar que el RUT tenga formato correcto (12345678-K)
            if (string.IsNullOrEmpty(loginDto.Rut) || !loginDto.Rut.Contains('-'))
            {
                return Results.BadRequest("Formato de RUT inválido. Use formato: 12345678-K");
            }

            // Extraer solo la parte numérica del RUT (antes del guión)
            var rutParts = loginDto.Rut.Split('-');

            
            int rutNumerico;
            if (rutParts.Length != 2 || !int.TryParse(rutParts[0], out rutNumerico))
            {
                return Results.BadRequest("Formato de RUT inválido. Use formato: 12345678-K");
            }

            
            var usuario = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Rut == rutNumerico);

            if (usuario == null || loginDto.Password.Hash() != usuario.Clave)
            {
                return Results.Unauthorized();
            }

            // Éxito - retornar información del usuario
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
            return Results.BadRequest($"Error durante el login: {ex.Message}");
        }
    }

    // ====================================================================================================================
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
}