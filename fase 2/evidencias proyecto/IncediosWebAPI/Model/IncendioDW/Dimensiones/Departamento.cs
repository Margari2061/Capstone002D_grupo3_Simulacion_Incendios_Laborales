using IncediosWebAPI.Model.IncendioDB.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDW.Dimensiones
{
    public class DepartamentoDW
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        
        public int SedeId { get; set; } // FK a Sede (desnormalizado)

        [MaxLength(100)]
        public string SedeNombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? SedeRegion { get; set; }

        // Propiedades para análisis
        public int TotalUsuarios { get; set; }
        public int TotalPartidas { get; set; }
        public double TasaExitoPromedio { get; set; }

        // Navigation properties
        public List<UsuarioDW> Usuarios { get; set; } = new();
    }
}