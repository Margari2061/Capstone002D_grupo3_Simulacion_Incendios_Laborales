public class RunDataTransfer
{
    public int Id { get; set; }
    public RunResults Resultado { get; set; }
    public int FuegosApagados { get; set; }
    public int ExtintoresUsados { get; set; }

    private bool _alarm;
    public bool UsoAlarma 
    {
        get => _alarm;
        set
        {
            if(!_alarm)
            {
                _alarm = value;
                return;
            }
            Desasosiego++;
        }
    }

    private bool _uniform;
    public bool UsoUniforme 
    {
        get => _uniform;
        set
        {
            if(!_uniform)
            {
                _uniform = value;
                return;
            }
            Desasosiego++;
        }
    }

    public int UsoInadecuadoExtintores { get; set; }
    public int Heridas { get; set; }
    public int Desasosiego { get; set; }

    public RunDataTransfer()
    {
        Id = 0;
        Resultado = RunResults.EnProgreso;
        FuegosApagados = 0;
        ExtintoresUsados = 0;
        UsoAlarma = false;
        UsoUniforme = false;
        UsoInadecuadoExtintores = 0;
        Heridas = 0;
        Desasosiego = 0;
    }

    public RunResults FinishRun(bool escape, int maxFires)
    {
        Resultado = DefineResult(escape, maxFires);
        return Resultado;
    }

    private RunResults DefineResult(bool escape, int maxFires)
    {
        if(!escape)
            return RunResults.Muerte;

        if(ExtintoresUsados == 0)
            return RunResults.EscapeInmediato;

        if (FuegosApagados < maxFires)
            return RunResults.EscapeSeguro;

        if(UsoUniforme && FuegosApagados == maxFires)
            return RunResults.CondicionesCumplidas;

        return RunResults.EscapeTardio;
    }
}

