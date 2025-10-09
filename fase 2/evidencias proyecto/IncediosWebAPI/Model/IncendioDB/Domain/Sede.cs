using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDB.Domain
{
    public class Sede
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RutEmpresa { get; set; }

        [ForeignKey(nameof(RutEmpresa))]
        public Empresa? Empresa { get; set; }

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
        public List<Departamento> Departamentos { get; set; } = [];
    }
}
