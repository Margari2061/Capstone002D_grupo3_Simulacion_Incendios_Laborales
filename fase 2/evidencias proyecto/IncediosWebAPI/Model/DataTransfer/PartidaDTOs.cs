using Microsoft.AspNetCore.Components.Sections;
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
    }

    public class PartidaFinalizarDTO
    { 
        public int Id { get; set; }

        public TimeSpan TiempoJugado { get; set; }
                    
        public ResultadosPartida Resultado { get; set; }
        
        public int FuegosApagados { get; set; }

        public int ExtintoresUsados {  get; set; }

        public int UsoInadecuadoExtintores {  get; set; }

        public bool UsoAlarma {  get; set; }

        public bool UsoUniforme {  get; set; }

        public int Heridas {  get; set; }
        
        public int Desasosiego {  get; set; }
    }
}