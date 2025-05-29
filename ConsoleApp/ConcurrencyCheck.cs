using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class ConcurrencyCheck
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            //config.LogTo(Console.WriteLine);

            Context context = new Context(config.Options);
            Order order = new Order { Name = "Zamówienie #15" };
            ConcurrencyToken(config, context, order);
            RowVersion(config, context, order);
            ConflictResolve(config, context, order);

        }

        private static void ConflictResolve(DbContextOptionsBuilder<Context> config, Context context, Order order)
        {
            var product = new Product { Name = "Produkt #5", Price = 50.0m, Order = order };
            context.Attach(order);
            context.Add(product);
            context.SaveChanges();

            product.Price = product.Price * 1.1m;

            Task.Run(() =>
            {
                using var context = new Context(config.Options);
                var product2 = context.Set<Product>().Find(product.Id);
                product2.Price = product2.Price + 10;
                context.SaveChanges();
            }).Wait();


            var saved = false;
            while (!saved)
            {
                try
                {
                    context.SaveChanges();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    foreach (var entry in ex.Entries)
                    {
                        //wartości jakie chcemy wprowadzić do bazy danych
                        var currentValues = entry.CurrentValues;
                        //wartości jakie mamy w kontekście (jakie pobraliśmy)
                        var originalValues = entry.OriginalValues;
                        //wartości jakie mamy w bazie danych
                        var databaseValues = entry.GetDatabaseValues();

                        switch (entry.Entity)
                        {
                            case Product p:
                                var property = currentValues.Properties.Single(x => x.Name == nameof(Product.Price));
                                //var currentPrice = property.CurrentValue;
                                var currentPrice = (decimal)currentValues[nameof(Product.Price)];
                                //var originalPrice = property.OriginalValue;
                                var originalPrice = (decimal)originalValues[nameof(Product.Price)];
                                var databasePrice = (decimal)databaseValues[nameof(Product.Price)];

                                currentPrice = databasePrice + (currentPrice - originalPrice);

                                currentValues[property] = currentPrice;
                                break;
                        }

                        entry.OriginalValues.SetValues(databaseValues); //ustawiamy wartości oryginalne na wartości z bazy danych, aby uniknąć kolejnego konfliktu konkurencji
                    }

                }
            }
        }

        private static void RowVersion(DbContextOptionsBuilder<Context> config, Context context, Order order)
        {
            var product = new Product { Name = "Produkt #4", Price = 40.0m, Order = order };

            context.Attach(order);
            context.Add(product);

            context.SaveChanges();

            Task.Run(() =>
            {
                using var context = new Context(config.Options);
                var product = context.Set<Product>().First();
                product.Name = "Produkt #4 - zmiana nazwy"; //zmiana nazwy, aby wywołać konflikt konkurencji
                context.SaveChanges();
            }).Wait();


            product.Price = 50.0m;

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
            }
            context.ChangeTracker.Clear();
        }

        private static void ConcurrencyToken(DbContextOptionsBuilder<Context> config, Context context, Order order)
        {
            context.Add(order);
            context.SaveChanges();

            Task.Run(() =>
            {
                using var context = new Context(config.Options);
                var order = context.Set<Order>().First();
                order.OrderDate = DateTime.Now.AddDays(-1); //zmiana OrderDate, aby wywołać konflikt konkurencji
                order.Name = "Zamówienie #15 - zmiana nazwy"; //zmiana nazwy, aby wywołać konflikt konkurencji
                context.SaveChanges();
            }).Wait();


            order.OrderDate = DateTime.Now.AddDays(1); //zmiana OrderDate, aby wywołać konflikt konkurencji
            order.Name = "Zamówienie #15 - kolejna zmiana nazwy";
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
            }
            context.ChangeTracker.Clear();
        }
    }
}
