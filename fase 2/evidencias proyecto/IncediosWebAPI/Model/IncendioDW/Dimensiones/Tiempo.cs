using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDW.Dimensiones
{
    public class Tiempo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public int Dia {  get; set; }
        public int Mes {  get; set; }
        public int Ano {  get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public string NombreDiaSemana { get; set; } = string.Empty;
        public int SemanaAno { get; set; }
        public int Trimestre { get; set; }
        public bool EsFinDeSemana { get; set; }
        public bool EsFeriado { get; set; }
    }
}
