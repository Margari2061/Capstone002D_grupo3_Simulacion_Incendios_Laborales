using System.ComponentModel.DataAnnotations;

namespace IncediosWebAPI.Model.DataTransfer
{
    // Para crear nuevas partidas desde el juego
    public class PartidaCreateDTO
    {
        [Required]
        public int RutUsuario { get; set; }

        [Required]
        public int Nivel { get; set; }

        [Required]
        public int TiempoJugadoSegundos { get; set; }

        [Required]
        public char Resultado { get; set; } // 0,1,2,3,4 según tu dominio

        [Required]
        public int FuegosApagados { get; set; }

        [Required]
        public int ExtintoresUsados { get; set; }

        [Required]
        public bool UsoAlarma { get; set; }

        [Required]
        [Range(0, 100)]
        public int Heridas { get; set; }

        [Required]
        [Range(0, 100)]
        public int Desasosiego { get; set; }
    }
}