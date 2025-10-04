using System.ComponentModel.DataAnnotations;

namespace IncediosWebAPI.Model.Domain
{
    public class Nivel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        /*[MaxLength(500)]
        public string? Descripcion { get; set; }*/

        [Required]
        public int FuegosApagadosEsperados { get; set; }

        [Required]
        public int ExtintoresUsadosEsperados { get; set; }

        [Required]
        public bool UsoAlarmaEsperado { get; set; }

        // Navigation property
        public virtual ICollection<Partida> Partidas { get; set; } = new List<Partida>();
    }
}