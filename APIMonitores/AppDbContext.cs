namespace APIMonitores
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<APIMonitores.Models.Monitor> Monitores { get; set; }
        public DbSet<APIMonitores.Models.Horario> Horarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Monitor>()
                .HasMany(m => m.Horarios)
                .WithOne(h => h.Monitor)
                .HasForeignKey(h => h.IdMonitor)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}