using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Usado para [Key] se você preferir
using System.ComponentModel.DataAnnotations.Schema;

namespace APIMonitores.Models
{
    [Table("Monitores")]
    public class AlunoMonitor

    {
        [Key] // Anotação para indicar que IdMonitor é a chave primária
        public int IdMonitor { get; set; }

        public required string RA { get; set; }

        public required string Nome { get; set; }

        public required string Apelido { get; set; }

        // Propriedade de navegação para Horarios
        public ICollection<Horario> Horarios { get; set; } = new List<Horario>();
    }
}