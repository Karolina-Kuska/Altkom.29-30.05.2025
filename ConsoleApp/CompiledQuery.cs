using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Diagnostics;

namespace ConsoleApp
{
    internal class CompiledQuery
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            Transactions.Run(config, false);

            config.LogTo(Console.WriteLine);

            Stopwatch timer1;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer1 = System.Diagnostics.Stopwatch.StartNew();
                var product = context.Set<Product>()
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Products)
                    .Where(x => x.Id % 2 == 0)
                    .Where(x => x.Order.Id % 2 != 0)
                    .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(5))
                    .OrderByDescending(x => x.Order.OrderDate)
                    .First();
                timer1.Stop();
            }

            Stopwatch timer2;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer2 = System.Diagnostics.Stopwatch.StartNew();
                var product = context.Set<Product>()
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Products)
                    .Where(x => x.Id % 2 == 0)
                    .Where(x => x.Order.Id % 2 != 0)
                    .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(10))
                    .OrderByDescending(x => x.Order.OrderDate)
                    .First();
                timer2.Stop();
            }

            Stopwatch timer3;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer3 = System.Diagnostics.Stopwatch.StartNew();
                var product = context.Set<Product>()
                    .Include(x => x.Order)
                    .ThenInclude(x => x.Products)
                    .Where(x => x.Id % 2 == 0)
                    .Where(x => x.Order.Id % 2 != 0)
                    .Where(x => x.Order.OrderDate < DateTime.Now.AddDays(15))
                    .OrderByDescending(x => x.Order.OrderDate)
                    .First();
                timer3.Stop();
            }

            Stopwatch timer4;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer4 = System.Diagnostics.Stopwatch.StartNew();
                var product = Context.GetProductsByDateTime(context, 5); //wywołanie skompilowanego zapytania
                timer4.Stop();
            }

            Stopwatch timer5;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer5 = System.Diagnostics.Stopwatch.StartNew();
                var product = Context.GetProductsByDateTime(context, 10);
                timer5.Stop();
            }


            Stopwatch timer6;
            using (var context = new Context(config.Options))
            {
                Console.Clear();
                timer6 = System.Diagnostics.Stopwatch.StartNew();
                var product = Context.GetProductsByDateTime(context, 15);
                timer6.Stop();
            }

            Console.Clear();
            Console.WriteLine($"Czas wykonania zwykłego zapytania 1: {timer1.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonania zwykłego zapytania 2: {timer2.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonania zwykłego zapytania 3: {timer3.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonania skompilowanego zapytania 1: {timer4.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonania skompilowanego zapytania 2: {timer5.ElapsedMilliseconds} ms");
            Console.WriteLine($"Czas wykonania skompilowanego zapytania 3: {timer6.ElapsedMilliseconds} ms");
        }
    }
}
