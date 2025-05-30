using DAL.Conventions;
using DAL.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Models;

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


        public static Func<Context, int, Product> GetProductsByDateTime { get; } =
            EF.CompileQuery((Context context, int addDays) =>
                context.Set<Product>()
                        .Include(x => x.Order)
                        .ThenInclude(x => x.Products)
                        .Where(x => x.Id % 2 == 0)
                        .Where(x => x.Order.Id % 2 != 0)
                        .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(addDays))
                        .OrderByDescending(x => x.Order.OrderDate)
                        .First()
            );

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(x => x.UseNetTopologySuite());
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

            //możemy wpływać na sposób dostępu do właściwości w modelu, np. preferując dostęp przez właściwości zamiast pól
            modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferProperty);
            //domyślne ustawienie to PropertyAccessMode.FieldDuringConstruction, czyli dostęp przez pola podczas tworzenia obiektu, a potem przez właściwości
            //modelBuilder.UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);

            modelBuilder.Model.GetEntityTypes()
                .SelectMany(x => x.GetProperties())
                .Where(x => x.ClrType == typeof(string))
                .Where(x => x.PropertyInfo?.CanWrite ?? false)
            .ToList()
            .ForEach(x => x.SetValueConverter(new ObfuscationConverter()));
        }

        public bool RandomFail { get; set; }

        public override int SaveChanges()
        {
            if (RandomFail && Random.Shared.Next(1, 25) == 1)
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
