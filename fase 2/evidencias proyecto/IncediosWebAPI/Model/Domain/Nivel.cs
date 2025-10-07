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

        [Required]
        public int FuegosApagadosEsperados { get; set; }

        [Required]
        public int ExtintoresUsadosEsperados { get; set; }

        [Required]
        public int UsoAlarmaEsperado { get; set; } // ← CAMBIÓ de bool a int
    }
}
        