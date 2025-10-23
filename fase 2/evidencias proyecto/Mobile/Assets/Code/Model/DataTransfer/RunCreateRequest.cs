using System;

public class RunCreateRequest
{
    public int RutUsuario { get; set; }
    public int Nivel { get; set; }
}

public class RunCreateResponse
{
    public int PartidaId { get; set; }
    public string Mensaje { get; set; }
    public DateTime FechaInicio { get; set; }
}