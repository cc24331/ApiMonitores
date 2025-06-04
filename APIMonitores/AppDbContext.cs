using Microsoft.EntityFrameworkCore;
using APIMonitores.Models; // << Garanta que esta linha está presente e correta!

namespace APIMonitores.Data // << AJUSTE ESTE NAMESPACE SE SEU ARQUIVO ESTÁ NA PASTA DATA
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<APIMonitores.Models.Monitor> Monitores { get; set; } // CORREÇÃO AQUI
        public DbSet<APIMonitores.Models.Horario> Horarios { get; set; }  // CORREÇÃO AQUI

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aqui dentro, 'Monitor' e 'Horario' já são desambiguados pelo using APIMonitores.Models;
            modelBuilder.Entity<Monitor>()
                .HasMany(m => m.Horarios)
                .WithOne(h => h.Monitor)
                .HasForeignKey(h => h.IdMonitor)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}