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


    // ==================== USUARIOS ====================

    // POST /api/usuarios/login - Autenticar usuario
    public static async Task<IResult> LoginUsuario(UsuarioLoginDTO loginDto, IncendioContext context)
    {
        try
        {
            int rutDTO = int.Parse(loginDto.Rut.Split('-')[0]);
            // Buscar usuario por email
            var usuario = await context.Usuarios

                .FirstOrDefaultAsync(u => u.Rut == rutDTO);

            if (usuario == null || loginDto.Password.Hash() != usuario.Clave)
            {
                return Results.Unauthorized();
            }

            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Error durante el login: {ex.Message}");
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
}