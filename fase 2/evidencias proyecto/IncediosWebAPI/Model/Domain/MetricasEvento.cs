using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.Domain
{
    public class MetricasEvento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        public bool ProtocolosSeguidos { get; set; }

        [Required]
        [Range(0, 100)]
        public int ContencionEstragos { get; set; } // Porcentaje de contención

        [Required]
        [Range(0, 1000)]
        public int Lesionados { get; set; }

    }
}