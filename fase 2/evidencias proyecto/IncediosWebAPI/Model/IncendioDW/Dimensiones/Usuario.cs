using IncediosWebAPI.Model.IncendioDB.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDW.Dimensiones
{
    public class UsuarioDW
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Rut { get; set; }

        [Required]
        public char Dv { get; set; }

        [Required, MaxLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Genero { get; set; } = string.Empty; // Desnormalizado como string

        public int Edad { get; set; } // Calculado desde FechaNacimiento

        public int Piso { get; set; }

        public bool EsMonitor { get; set; }

        
        public int DepartamentoId { get; set; } // FK a Departamento (desnormalizado)

        [MaxLength(255)]
        public string DepartamentoNombre { get; set; } = string.Empty;

        // ================  FKs adicionales desnormalizadas =====================
        public int SedeId { get; set; }
        public string SedeNombre { get; set; } = string.Empty;

        // Roles desnormalizados
        public bool EsPlayer { get; set; }
        public bool EsAdmin { get; set; }

        // Métricas de desempeño (para análisis rápido)
        public int TotalPartidas { get; set; }
        public int PartidasExitosas { get; set; }
        public double TasaExito { get; set; }
        public double TiempoPromedioPartida { get; set; }
        public DateTime? FechaUltimaPartida { get; set; }

        // Navigation properties
        public List<PartidaDW> Partidas { get; set; } = new();
    }
}