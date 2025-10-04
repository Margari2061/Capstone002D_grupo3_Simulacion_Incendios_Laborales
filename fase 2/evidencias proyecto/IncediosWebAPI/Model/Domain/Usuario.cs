using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.Domain
{
    public class Usuario
    {
        [Key]
        public int Rut { get; set; }

        [Required]
        [MaxLength(1)]
        public char Dv { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Mail { get; set; } = string.Empty;

        [Required]
        [MaxLength(1)]
        public char Genero { get; set; } = 'N'; // M, F, N

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [MaxLength(255)]
        public string Clave { get; set; } = string.Empty;

        public int Piso { get; set; }

        [Required]
        public bool EsMonitor { get; set; } = false;

        [Required]
        [ForeignKey("Departamento")]
        public int IdDepartamento { get; set; }

        // Navigation properties
        public virtual Departamento Departamento { get; set; } = null!;
        public virtual ICollection<Partida> Partidas { get; set; } = new List<Partida>();
    }
}