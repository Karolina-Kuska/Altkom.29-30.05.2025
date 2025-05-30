using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class Transactions
    {

        public static void Run(DbContextOptionsBuilder<Context> config, bool randomFail = true)
        {
            var products = Enumerable.Range(100, 50).Select(x => new Product { Name = $"Produkt #{x}", Price = x * 10.0m, ProductDetails = new ProductDetails { Depth = x * 0.1f, Height = x * 0.2f, Width = x * 0.3f, Weight = x * 0.4f } }).ToList();
            var orders = Enumerable.Range(1, 5).Select(x => new Order { Name = $"Zamówienie #{x}", OrderDate = DateTime.Now.AddDays(3.21 * x) }).ToList();

            using var context = new Context(config.Options);
            context.RandomFail = randomFail;

            using (var transaction = context.Database.BeginTransaction())
            {
                for (int i = 0; i < orders.Count; i++)
                {
                    string savePoint = $"SavePoint_{i + 1}";
                    transaction.CreateSavepoint(savePoint); //tworzy savepoint, do którego można cofnąć zmiany w przypadku błędu
                    try
                    {
                        var subproducts = products.Skip(i * 10).Take(10).ToList();

                        foreach (var product in subproducts)
                        {
                            context.Add(product);
                            context.SaveChanges();
                        }

                        var order = orders[i];
                        order.Products = subproducts;
                        context.Add(order);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
                        transaction.RollbackToSavepoint(savePoint); //cofa zmiany do ostatniego savepoint
                    }
                    //czyścimy zmiany w kontekście, aby nie były widoczne w kolejnych iteracjach dla savepoint
                    context.ChangeTracker.Clear();
                }

                transaction.Commit(); //zapisuje zmiany w bazie danych, jeśli nie wystąpił błąd
            }
        }
    }
}
