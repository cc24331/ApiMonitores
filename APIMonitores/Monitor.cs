namespace SuaAPI.Models
{
    public class Monitor
    {
        public int IdMonitor { get; set; }
        public required string RA { get; set; }
        public required string Nome { get; set; }
        public required string Apelido { get; set; }

        public ICollection<Horario>? Horarios { get; set; }
    }
}
