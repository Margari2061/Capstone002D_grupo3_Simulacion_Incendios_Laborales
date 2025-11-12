using IncediosWebAPI.Model.IncendioDB.Domain;
using System.ComponentModel.DataAnnotations;

namespace IncediosWebAPI.Model.DataTransfer
{
    // Para registrar nuevos usuarios
    public class UsuarioRegistroDTO
    {
        [Required]
        public string Rut { get; set; } = "";

        [Required, MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public Generos Genero { get; set; }

        [Required]
        public DateOnly FechaNacimiento { get; set; }

        [Required]
        [MaxLength(255)]
        public string Clave { get; set; } = string.Empty;

        public int Piso { get; set; }

        public bool EsMonitor { get; set; } = false;

        [Required]
        public int IdDepartamento { get; set; }

        public bool EsPlayer { get; set; }
        public bool EsAdmin { get; set; }
    }

    // Para hacer login
    public class UsuarioLoginDTO
    {
        [Required]
        public string Rut { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Para enviar datos de usuario (sin información sensible)
    public class UsuarioResponseDTO
    {
        public int Rut { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public Generos Genero { get; set; }
        public bool EsMonitor { get; set; }
        public int IdDepartamento { get; set; }
    }

    public class UsuarioDetails
    {
        public Usuario User { get; set; } = new();
        public int PlayedRuns { get; set; }
        public string TimePlayed { get; set; } = "";
        public double AverageExtintionRatio { get; set; }
        public double DamagePerRuns { get; set; }
    }
}