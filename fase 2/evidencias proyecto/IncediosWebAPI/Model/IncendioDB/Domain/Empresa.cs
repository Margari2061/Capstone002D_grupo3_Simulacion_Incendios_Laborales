// DEFINE LA TABLA EMPRESA EN BASE DE DATOS

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncediosWebAPI.Model.IncendioDB.Domain;

public class Empresa
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Rut { get; set; }

    [Required]
    public char Dv { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = "";


    [MaxLength(100)]
    public string Rubro { get; set; } = "";

    // Navigation property
    public List<Sede> Sedes { get; set; } = [];
    public List<MetricasEvento> Metricas { get; set; } = [];

}