using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace IncediosWebAPI.Model.IncendioDB.Domain
{
    [Index(nameof(IdSede), nameof(Nombre), IsUnique = true)]   //Evitar departamentos duplicados en la misma sede.
    public class Departamento
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public int IdSede { get; set; }

        [ForeignKey(nameof(IdSede))]
        public Sede? Sede { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        // Navigation properties
        public List<Usuario> Usuarios { get; set; } = [];
    }
}
