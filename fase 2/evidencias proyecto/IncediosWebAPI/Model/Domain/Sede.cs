
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.Domain
{
    public class Sede
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Empresa")]
        public int RutEmpresa { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Direccion { get; set; }

        [MaxLength(100)]
        public string? Comuna { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

        // Navigation properties
        public virtual Empresa Empresa { get; set; } = null!;
        public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
    }
}
