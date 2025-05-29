using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class GlobalFilters
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            ShadowProperty.Run(config);

            using (var context = new Context(config.Options))
            {

                var products = context.Set<Product>().ToList();

                foreach (var product in products.Where(x => x.Id % 2 == 0))
                {
                    context.Entry(product).Property<bool>("IsDeleted").CurrentValue = true;
                }
                context.SaveChanges();
                context.ChangeTracker.Clear();

                products = context.Set<Product>().ToList();

                foreach (var product in products)
                {
                    Console.WriteLine($"{product.Id} {product.Name} {product.Price} {context.Entry(product).Property<bool>("IsDeleted").CurrentValue}");
                }

                /*var order = context.Set<Order>().Include(x => x.Products.Where(xx => EF.Property<bool>(xx, "IsDeleted") == false))
                    .Where(xx => EF.Property<bool>(xx, "IsDeleted") == false).ToList();*/

                var order = context.Set<Order>().Include(x => x.Products).ToList();

                foreach (var o in order)
                {
                    Console.WriteLine($"{o.Id} {o.Name} {o.Products.Count}");
                    foreach (var p in o.Products)
                    {
                        Console.WriteLine($"\t{p.Id} {p.Name} {p.Price} {context.Entry(p).Property<bool>("IsDeleted").CurrentValue}");
                    }
                }

                //wyłączenie filtrów globalnych w zapytaniu
                var allProducts = context.Set<Product>().IgnoreQueryFilters().ToList();

                foreach (var product in allProducts)
                {
                    Console.WriteLine($"{product.Id} {product.Name} {product.Price} {context.Entry(product).Property<bool>("IsDeleted").CurrentValue}");
                }
            }
        }
    }
}
