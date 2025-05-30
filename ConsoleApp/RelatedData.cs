using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class RelatedData
    {

        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            Transactions.Run(config);

            config.LogTo(Console.WriteLine);

            using (var context = new Context(config.Options))
            {
                Console.Clear();
                //Eager loading - ładowanie danych razem z głównym obiektem
                //var products = context.Set<Product>().Include(x => x.Order).ToList();
                //var products = context.Set<Product>().Include(x => x.Order).ThenInclude(x => x.Products).ToList();

                //AsSplitQuery - ładowanie danych w wielu zapytaniach
                var products = context.Set<Product>().AsSplitQuery().Include(x => x.Order).ThenInclude(x => x.Products).ToList();
            }


            using (var context = new Context(config.Options))
            {
                var product = context.Set<Product>().First();
                //Explicit loading - ładowanie danych na żądanie

                context.Entry(product).Reference(x => x.Order).Load(); //ładowanie pojedynczego obiektu

                context.Entry(product.Order).Collection(x => x.Products).Load();
            }

            using (var context = new Context(config.Options))
            {
                var orders = context.Set<Order>().Where(x => x.Id % 2 == 0).ToList(); //ładowanie wszystkich zamówień z produktami

                //context.Set<Product>().Load(); //ładowanie wszystkich produktów

                context.Set<Product>().Where(x => orders.Select(xx => xx.Id).Contains(x.Order.Id)).Load();
            }

            Product lazyProduct;
            //config.UseLazyLoadingProxies(); //włączenie lazy loadingu na podstawie proxy
            using (var context = new Context(config.Options))
            {
                //lazy loading - ładowanie danych przy pierwszym dostępie do właściwości
                lazyProduct = context.Set<Product>().First();

                    Console.WriteLine(lazyProduct.Order.Name);

                context.ChangeTracker.Clear();

                lazyProduct = context.Set<Product>().First();
            }

                Console.WriteLine(lazyProduct.Order.Name);
        }
    }
}
