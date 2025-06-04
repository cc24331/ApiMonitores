namespace SuaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Monitor> Monitores { get; set; }
        public DbSet<Horario> Horarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            modelBuilder.Entity<Models.Monitor>()
                .HasKey(m => m.IdMonitor);

            modelBuilder.Entity<Horario>()
                .HasKey(h => h.IdHorario);

            modelBuilder.Entity<Horario>()
                .HasOne(h => h.Monitor)
                .WithMany(m => m.Horarios)
                .HasForeignKey(h => h.IdMonitor);
        }
    }
}
