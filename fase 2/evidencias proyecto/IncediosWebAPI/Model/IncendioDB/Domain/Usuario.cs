using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDB.Domain
{
    public class Usuario
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Rut { get; set; }

        [Required]
        public char Dv { get; set; }

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

        [ForeignKey(nameof(IdDepartamento))]
        public Departamento? Departamento { get; set; }

        public AppRoles Roles { get; set; }

        // Navigation properties
        public List<Partida> Partidas { get; set; } = [];
    }
}