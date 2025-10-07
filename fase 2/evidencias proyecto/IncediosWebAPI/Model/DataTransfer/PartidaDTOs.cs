using System.ComponentModel.DataAnnotations;

namespace IncediosWebAPI.Model.DataTransfer
{
    // ==================== PARA COMENZAR PARTIDA ====================
    public class PartidaCreateDTO
    {
        [Required]
        public int RutUsuario { get; set; }

        [Required]
        public int Nivel { get; set; }

 
    }

    // ==================== PARA FINALIZAR PARTIDA ====================
    public class PartidaFinalizarDTO
    {
        [Required]
        public int PartidaId { get; set; }

        [Required]
        public int TiempoJugadoSegundos { get; set; }

        [Required]
        public char Resultado { get; set; } // 0,1,2,3,4 

        [Required]
        public int FuegosApagados { get; set; }

        [Required]
        public int ExtintoresUsados { get; set; }

        [Required]
        public int UsoInadecuadoExtintores { get; set; }

        [Required]
        public bool UsoAlarma { get; set; }

        [Required]
        public bool UsoUniforme { get; set; }

        [Required]
        [Range(0, 100)]
        public int Heridas { get; set; } // 0 = sin daño (full vida)

        [Required]
        [Range(0, 100)]
        public int Desasosiego { get; set; } // 0 = completamente calmado
    }

    // ==================== PARA CONSULTAR PARTIDAS ====================
    public class PartidaResponseDTO
    {
        public int Id { get; set; }
        public int NivelId { get; set; }
        public string NombreNivel { get; set; } = string.Empty;
        public int TiempoJugadoSegundos { get; set; }
        public char Resultado { get; set; }
        public string DescripcionResultado { get; set; } = string.Empty;
        public int FuegosApagados { get; set; }
        public int ExtintoresUsados { get; set; }
        public int UsoInadecuadoExtintores { get; set; }
        public bool UsoAlarma { get; set; }
        public bool UsoUniforme { get; set; }
        public double RatioExtincion { get; set; }
        public int Heridas { get; set; } // 0 = jugador perfecto (sin daño)
        public int Desasosiego { get; set; } // 0 = jugador calmado
        public DateTime Fecha { get; set; }
    }
}