using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDW.Dimensiones
{
    public class TiempoDW
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


        //Generar ID por fecha
        public static int GenerarId(DateTime fecha)
        {
            return fecha.Year * 10000 + fecha.Month * 100 + fecha.Day;
        }


        // Constructor
        public TiempoDW(DateTime fecha)
        {
            Fecha = fecha.Date;
            Id = GenerarId(fecha);

            Dia = fecha.Day;
            Mes = fecha.Month;
            Ano = fecha.Year;
            NombreMes = fecha.ToString("MMMM");
            NombreDiaSemana = fecha.ToString("dddd");
            SemanaAno = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                fecha, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            Trimestre = (fecha.Month - 1) / 3 + 1;
            EsFinDeSemana = fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday;
            EsFeriado = false; 
        }

        // Constructor vacío para EF
        public TiempoDW() { }
    }
}
