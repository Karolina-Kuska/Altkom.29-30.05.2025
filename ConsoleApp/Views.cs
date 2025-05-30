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
    internal class Views
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            Transactions.Run(config, false);

            using var context = new Context(config.Options);

            var summary = context.Set<OrderSummary>().ToList();

            foreach (var item in summary)
            {
                Console.WriteLine($"Order ID: {item.Id}, Total Value: {item.TotalValue}, Order Date: {item.OrderDate}");
            }

        }
    }
}
