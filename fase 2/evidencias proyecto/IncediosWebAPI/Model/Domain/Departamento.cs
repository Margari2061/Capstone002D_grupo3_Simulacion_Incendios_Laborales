

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace IncediosWebAPI.Model.Domain
{
    [Index(nameof(IdSede), nameof(Nombre), IsUnique = true)]   //Evitar departamentos duplicados en la misma sede.
    public class Departamento
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Sede")]
        public int IdSede { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        // Navigation properties
        public virtual Sede Sede { get; set; } = null!;
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
