using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Models;

namespace ConsoleApp
{
    internal class ChangeTracker
    {
        public static void Run(DbContextOptions<Context> options)
        {
            using var context = new Context(options);
            //AutoDetectChangesEnabled dziala w przypadku wywołania SaveChanges, Entry
            context.ChangeTracker.AutoDetectChangesEnabled = true;

            Order order = new Order { Name = "Zamówienie #14" };
            Product product1 = new Product { Name = "Produkt #1", Price = 10.0m };
            order.Products.Add(product1);

            Console.WriteLine("Order przed dodaniem do kontekstu: " + context.Entry(order).State);
            Console.WriteLine("Product1 przed dodaniem do kontekstu: " + context.Entry(product1).State);

            //context.Set<Order>().Attach(order); //przypisuje Order do kontekstu i w zależności od wartości klucza głównego ustawia stan na Unchanged lub Added
            context/*.Set<Order>()*/.Add(order); //wymusza dodanie do kontekstu, nawet jeśli Order jest już w kontekście (stan Added)

            Console.WriteLine("Order po dodaniu do kontekstu: " + context.Entry(order).State);
            Console.WriteLine("Product1 po dodaniu do kontekstu: " + context.Entry(product1).State);

            context.SaveChanges();

            Console.WriteLine("Order po zapisie do bazy: " + context.Entry(order).State);
            Console.WriteLine("Product1 po zapisie do bazy: " + context.Entry(product1).State);

            order.OrderDate = DateTime.Now.AddDays(-1);
            var product2 = new Product { Name = "Produkt #2", Price = 20.0m };
            order.Products.Add(product2);

            Console.WriteLine("Order po modyfikacji: " + context.Entry(order).State);
            Console.WriteLine("Order.Name po modyfikacji order: " + context.Entry(order).Property(o => o.Name).IsModified);
            Console.WriteLine("Order.OrderDate po modyfikacji order: " + context.Entry(order).Property(o => o.OrderDate).IsModified);
            Console.WriteLine("Order.Products po modyfikacji order: " + context.Entry(order).Collection(o => o.Products).IsModified);

            Console.WriteLine("Product1 po modyfikacji order: " + context.Entry(product1).State);
            Console.WriteLine("Product2 po modyfikacji order: " + context.Entry(product2).State);

            order.Products.Remove(product1);
            Console.WriteLine("Product1 po usunięciu: " + context.Entry(product1).State);

            context.ChangeTracker.DetectChanges(); //wymuszenie wykrycia zmian w kontekście, aby zmiany były widoczne w Entry

            context.SaveChanges();

            Console.WriteLine("Order po zapisie do bazy: " + context.Entry(order).State);
            Console.WriteLine("Product1 po zapisie do bazy: " + context.Entry(product1).State);
            Console.WriteLine("Product2 po zapisie do bazy: " + context.Entry(product2).State);

            context.ChangeTracker.Clear(); //wyczyszczenie snapshotów stanów obiektów w kontekście (kolekcji OriginalValus i CurrentValues)

            var product3 = new Product { Name = "Produkt #3", Price = 30.0m, Order = order };
            //context.Add(product3); //nie możemy używać Add, ponieważ Order z Id = 1 nie jest w kontekście, więc EF Core nie wie, co zrobić z Orderem.
            context.Attach(product3); //Możemy użyć Attach, wtedy Order będzie miał stan Unchanged, a Product3 będzie miał stan Added.

            Console.WriteLine("Order po dodaniu do kontekstu: " + context.Entry(product3.Order).State);
            Console.WriteLine("Product3 po dodaniu do kontekstu: " + context.Entry(product3).State);
            context.SaveChanges();
            Console.WriteLine("Order po zapisie do bazy: " + context.Entry(product3.Order).State);
            Console.WriteLine("Product3 po zapisie do bazy: " + context.Entry(product3).State);

            context.ChangeTracker.Clear();

            context.Attach(order);
            order.Products.Remove(product2);
            order.Name = "Zamówienie #14 - zmodyfikowane";

            //istnieje możliwość wyłączenia modyfikacji poszczególnych właściwości obiektu, np. Name, OrderDate, Products
            context.Entry(order).Property(o => o.Name).IsModified = false;

            Console.WriteLine("Order po modyfikacji: " + context.Entry(order).State);
            Console.WriteLine("Order.Name po modyfikacji order: " + context.Entry(order).Property(o => o.Name).IsModified);
            Console.WriteLine("Order.OrderDate po modyfikacji order: " + context.Entry(order).Property(o => o.OrderDate).IsModified);
            Console.WriteLine("Order.Products po modyfikacji order: " + context.Entry(order).Collection(o => o.Products).IsModified);

            Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);


            context.ChangeTracker.DetectChanges();

            //ręczna zmiana stanu obiektu na Unchanged, aby zmiana nie była zapisana do bazy danych
            context.Entry(product2).State = EntityState.Unchanged;
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);

            context.SaveChanges();


            order.Name = "Zamówienie #14 - zmodyfikowane ponownie";
            order.Products.First().Name = "Produkt - zmodyfikowany";


            context.ChangeTracker.DetectChanges(); //DebugView nie wywołuje DetectChanges, więc musimy to zrobić ręcznie
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);

            //odwołanie się do Entry - wywołuje DetectChanges, więc nie musimy tego robić ręcznie
            Console.WriteLine("Order.Name po modyfikacji order: " + context.Entry(order).Property(o => o.Name).IsModified);

            context.SaveChanges();
        }


        public static void TrackingProxies(DbContextOptionsBuilder<Context> options)
        {
            options.UseChangeTrackingProxies();

            using var context = new Context(options.Options)
            {
                ChangeTracker = { AutoDetectChangesEnabled = false }
            };

            //var order = new Order { Name = "Zamówienie #15" };
            var order = context.CreateProxy<Order>();
            order.Name = "Zamówienie #15";
            var product1 = context.CreateProxy<Product>(x => { x.Price = 1; x.Name = "Produkt #1"; });
            order.Products.Add(product1);

            context.Add(order);
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);

            context.SaveChanges();

            Console.WriteLine(context.ChangeTracker.DebugView.LongView);

            order.Name = "Zamówienie #15 - zmodyfikowane";
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);
            order.Products.Remove(product1);
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);
        }

        public static void ChangedNotification(DbContextOptions<Context> options)
        {
            using var context = new Context(options)
            {
                ChangeTracker = { AutoDetectChangesEnabled = false }
            };

            var order = new Order { Name = "Zamówienie #15" };
            order.Name = "Zamówienie #15";
            var product1 = new Product { Price = 1, Name = "Produkt #1" };
            order.Products.Add(product1);

            context.Add(order);
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);

            context.SaveChanges();

            Console.WriteLine(context.ChangeTracker.DebugView.LongView);

            order.Name = "Zamówienie #15 - zmodyfikowane";
            order.OrderDate = DateTime.Now.AddDays(-1);
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);
            order.Products.Remove(product1);
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);


            context.SaveChanges();
        }
    }
}
