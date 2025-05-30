using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class Context : DbContext
    {

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public Context()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.Snapshot); // Ustawienie domyślne strategii śledzenia zmian to Snapshot
            //modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications); 

            base.OnModelCreating(modelBuilder);
        }

        public bool RandomFail { get; set; }

        public override int SaveChanges()
        {
            if(RandomFail && Random.Shared.Next(1, 25) == 1)
            {
                throw new DbUpdateException("Losowy błąd podczas zapisu do bazy danych.");
            }

            return base.SaveChanges();
        }
    }
}
