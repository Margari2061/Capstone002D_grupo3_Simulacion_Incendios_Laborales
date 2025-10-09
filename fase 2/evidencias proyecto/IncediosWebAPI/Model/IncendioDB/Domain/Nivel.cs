using System.ComponentModel.DataAnnotations;

namespace IncediosWebAPI.Model.IncendioDB.Domain
{
    public class Nivel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public int FuegosApagadosEsperados { get; set; }

        [Required]
        public int ExtintoresUsadosEsperados { get; set; }

        [Required]
        public bool UsoAlarmaEsperado { get; set; }

        // Navigation property
        public List<Partida> Partidas { get; set; } = [];
    }
}