namespace IncediosWebAPI.Model.Domain
{
    public static class Dominios
    {
        // Dominio de Género
        public const char GeneroMasculino = 'M';
        public const char GeneroFemenino = 'F';
        public const char GeneroNoInforma = 'N';

        // Dominio de Resultados (según tu diagrama)
        public static class ResultadosPartida
        {
            public const char CondicionesCumplidas = '0';
            public const char EscapeSeguro = '1';
            public const char EscapeInmediato = '2';
            public const char EscapeTardio = '3';
            public const char Muerte = '4';

            public static string GetDescripcion(char resultado)
            {
                return resultado switch
                {
                    '0' => "Condiciones Cumplidas",
                    '1' => "Escape seguro",
                    '2' => "Escape inmediato",
                    '3' => "Escape tardío",
                    '4' => "Muerte",
                    _ => "Desconocido"
                };
            }
        }
    }
}