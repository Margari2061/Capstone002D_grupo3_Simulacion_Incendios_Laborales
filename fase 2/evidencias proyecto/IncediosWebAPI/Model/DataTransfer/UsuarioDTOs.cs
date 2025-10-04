using System.ComponentModel.DataAnnotations;

namespace IncediosWebAPI.Model.DataTransfer
{
    // Para registrar nuevos usuarios
    public class UsuarioRegistroDTO
    {
        [Required]
        public int Rut { get; set; }

        [Required]
        public char Dv { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Mail { get; set; } = string.Empty;

        public char Genero { get; set; } = 'N'; // M, F, N

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public int Piso { get; set; }

        public bool EsMonitor { get; set; } = false;

        [Required]
        public int IdDepartamento { get; set; }
    }

    // Para hacer login
    public class UsuarioLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Mail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Para enviar datos de usuario (sin información sensible)
    public class UsuarioResponseDTO
    {
        public int Rut { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public char Genero { get; set; }
        public bool EsMonitor { get; set; }
        public int IdDepartamento { get; set; }
    }
}