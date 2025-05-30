using DAL.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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


            modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(x => x.ClrType == typeof(int))
                .Where(x => x.Name == "Key")
                .ToList()
                .ForEach(x =>
                {
                    x.IsNullable = false;
                    ((IMutableEntityType)x.DeclaringType).SetPrimaryKey(x);
                });

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

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            //configurationBuilder.Properties<DateTime>().HavePrecision(5);
            configurationBuilder.Conventions.Add(_ => new DateTimePrecisionConvention());
            configurationBuilder.Conventions.Add(_ => new PluralizeTableNameConvention());

            //configurationBuilder.Conventions.Remove(typeof(KeyDiscoveryConvention));
        }
    }
}
