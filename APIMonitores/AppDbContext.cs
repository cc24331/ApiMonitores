using Microsoft.EntityFrameworkCore; // Essencial para DbContext, DbSet, ModelBuilder
using APIMonitores.Models;          // Essencial para seus modelos Monitor e Horario

namespace APIMonitores
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // CORREÇÃO: Qualificando 'Monitor' com seu namespace completo
        public DbSet<APIMonitores.Models.Monitor> Monitores { get; set; }
        public DbSet<APIMonitores.Models.Horario> Horarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CORREÇÃO: Qualificando 'Monitor' com seu namespace completo
            modelBuilder.Entity<APIMonitores.Models.Monitor>()
                .HasMany(m => m.Horarios)
                .WithOne(h => h.Monitor)
                .HasForeignKey(h => h.IdMonitor)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}