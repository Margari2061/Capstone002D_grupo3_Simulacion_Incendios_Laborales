using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDB.Domain
{
    public class Partida
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RutUsuario { get; set; }
        [ForeignKey(nameof(RutUsuario))]
        public Usuario? Usuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        public int IdNivel { get; set; } // FK a Nivel.Id

        [ForeignKey(nameof(IdNivel))]
        public Nivel? Nivel { get; set; }

        [Required]
        public TimeSpan TiempoJugado { get; set; }

        [Required]
        public ResultadosPartida Resultado { get; set; }

        [Required]
        public int FuegosApagados { get; set; }

        [Required]
        public int ExtintoresUsados { get; set; }

        [Required]
        public bool UsoAlarma { get; set; }

        [Required]
        public bool UsoUniforme { get; set; }

        [Required]
        public int UsoInadecuadoExtintores {  get; set; }

        [Required]
        public int Heridas { get; set; }

        [Required]
        public int Desasosiego { get; set; }
    }
}