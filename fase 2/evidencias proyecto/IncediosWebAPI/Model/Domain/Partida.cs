using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.Domain
{
    public class Partida
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int RutUsuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("Nivel")]
        public int IdNivel { get; set; } // FK a Nivel.Id CAMBIÓ de "Nivel" a "IdNivel"

        [Required]
        public TimeSpan TiempoJugado { get; set; }

        [Required]
        [MaxLength(1)]
        public char Resultado { get; set; } // 0,1,2,3,4 según dominio

        [Required]
        public int FuegosApagados { get; set; }

        [Required]
        public int ExtintoresUsados { get; set; }

        [Required]
        public int UsoInadecuadoExtintores { get; set; } // NUEVOOO

        [Required]
        public bool UsoAlarma { get; set; }

        [Required]
        public bool UsoUniforme { get; set; } // NUEVOOO

        [Required]
        [Range(0, 100)]
        public int Heridas { get; set; }

        [Required]
        [Range(0, 100)]
        public int Desasosiego { get; set; }

        // Navigation properties
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual Nivel Nivel { get; set; } = null!; // Actualizar nombre


        // Propiedad calculada - Ratio de Extinción
        [NotMapped]
        public double RatioExtincion => ExtintoresUsados > 0 ?
        (double)FuegosApagados / ExtintoresUsados : 0;
    }
}
