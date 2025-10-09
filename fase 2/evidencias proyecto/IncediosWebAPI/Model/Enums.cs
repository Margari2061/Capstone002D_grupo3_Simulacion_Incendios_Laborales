using System.ComponentModel.DataAnnotations;

namespace IncediosWebAPI.Model;

public enum Generos
{
    [Display(Name = "No informa")] N,
    [Display(Name = "Masculino")] M,
    [Display(Name = "Femenino")] F
}

public enum ResultadosPartida
{
    EnProgreso,
    CondicionesCumplidas,
    EscapeSeguro,
    EscapeInmediato,
    EscapeTardio,
    Muerte
}