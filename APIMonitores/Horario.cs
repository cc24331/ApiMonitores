namespace APIMonitores.Models 
{
    public class Horario
    {
        public int IdHorario { get; set; }
        [Required] // Adicione Required se quiser
        public required int DiaSemana { get; set; }

        [Required]
        [MaxLength(10)]
        public required string HorarioTexto { get; set; }

        public int IdMonitor { get; set; }
        public Monitor? Monitor { get; set; }
    }
}