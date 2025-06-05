using System.ComponentModel.DataAnnotations; // Usado para [Key]
using System.ComponentModel.DataAnnotations.Schema; // Usado para [ForeignKey]

namespace APIMonitores.Models
{
    public class Horario
    {
        [Key] // Anotação para indicar que IdHorario é a chave primária
        public int IdHorario { get; set; }

        public int diaSemana { get; set; }

        public required string horario { get; set; }

        // Chave estrangeira
        public int IdMonitor { get; set; }

        // Propriedade de navegação para Monitor
        [ForeignKey("IdMonitor")] // Opcional, mas boa prática para clareza
        public AlunoMonitor Monitor { get; set; } = null!; // null! indica que não será nulo após carregamento do EF
    }
}