using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDB.Domain
{
    public class MetricasEvento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTimeOffset Fecha { get; set; } = DateTimeOffset.Now;

        [Required]
        public bool ProtocolosSeguidos { get; set; }

        [Required]
        public int CotizacionEstragos { get; set; } // Porcentaje de contención

        [Required]
        public int Lesionados { get; set; }

    }
}