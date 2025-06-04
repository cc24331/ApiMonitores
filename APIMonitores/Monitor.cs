using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // << ADICIONE ESTA LINHA (se for usar MaxLength, etc.)

namespace APIMonitores.Models // << AJUSTE PARA ESTE NAMESPACE
{
    public class Monitor
    {
        public int IdMonitor { get; set; }
        [MaxLength(20)] // Adicione MaxLength se quiser, lembre-se de usar using System.ComponentModel.DataAnnotations;
        public required string RA { get; set; }

        [MaxLength(100)]
        public required string Nome { get; set; }

        [MaxLength(50)]
        public required string Apelido { get; set; }
        public ICollection<Horario> Horarios { get; set; } = new List<Horario>();
    }
}