namespace SuaAPI.Models
{
    public class Horario
    {
        public int IdHorario { get; set; }
        public int DiaSemana { get; set; }
        public required string HorarioTexto { get; set; }

        // Chave estrangeira
        public int IdMonitor { get; set; }
        public Monitor? Monitor { get; set; }
    }
}
