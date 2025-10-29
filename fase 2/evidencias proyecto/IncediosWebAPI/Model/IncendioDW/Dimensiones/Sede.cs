using IncediosWebAPI.Model.IncendioDB.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDW.Dimensiones
{
    public class SedeDW
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Direccion { get; set; }

        [MaxLength(100)]
        public string? Comuna { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }

         
        public int EmpresaId { get; set; } // FK a la empresa

        [MaxLength(100)]
        public string? EmpresaNombre { get; set; }

        [MaxLength(100)]
        public string? RubroEmpresa { get; set; }

        // Propiedades para análisis
        public int TotalDepartamentos { get; set; }
        public int TotalUsuarios { get; set; }



        // ======= Quizas no es necesario ponerlo ========
        public List<DepartamentoDW> Departamentos { get; set; } = new();
    }
}