using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDW.Dimensiones
{
    public class PartidaDW
    {
        [Key]
        public int Id { get; set; }

        // FKs a dimensiones
        public int UsuarioRut { get; set; }
        public int TiempoId { get; set; }
        public int NivelId { get; set; }

        // Datos desnormalizados para performance
        [MaxLength(255)]
        public string UsuarioNombre { get; set; } = string.Empty;

        [MaxLength(255)]
        public string NivelNombre { get; set; } = string.Empty;

        // Métricas de la partida
        public int TiempoJugadoSegundos { get; set; }
        public string Resultado { get; set; } = string.Empty; // Desnormalizado
        public int FuegosApagados { get; set; }
        public int ExtintoresUsados { get; set; }
        public int UsoInadecuadoExtintores { get; set; }
        public bool UsoAlarma { get; set; }
        public bool UsoUniforme { get; set; }
        public int Heridas { get; set; }
        public int Desasosiego { get; set; }

        // Métricas calculadas para análisis
        public int PuntuacionContencion { get; set; }
        public bool ProtocolosSeguidos { get; set; }
        public bool HuboLesionados { get; set; }
        public double EficienciaExtintores { get; set; }

        // Navigation properties
        public UsuarioDW Usuario { get; set; } = null!;
        public TiempoDW Tiempo { get; set; } = null!;
    }
}