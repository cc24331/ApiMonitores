using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Usado para [Key] se você preferir
using System.ComponentModel.DataAnnotations.Schema;

namespace APIMonitores.Models
{
    [Table("Monitores")]
    public class AlunoMonitor

    {
        [Key]
        public int IdMonitor { get; set; }

        public required string RA { get; set; }

        public required string Nome { get; set; }

        public required string Apelido { get; set; }

        public ICollection<Horario> Horarios { get; set; } = new List<Horario>();
    }
}