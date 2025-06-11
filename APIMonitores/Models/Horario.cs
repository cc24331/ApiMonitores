using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; 

namespace APIMonitores.Models
{
    public class Horario
    {
        [Key] 
        public int IdHorario { get; set; }

        public String diaSemana { get; set; }  = string.Empty;

        public required string horario { get; set; }

        public int IdMonitor { get; set; }

        [ForeignKey("IdMonitor")] 
        public AlunoMonitor Monitor { get; set; } = null!; 
    }
}