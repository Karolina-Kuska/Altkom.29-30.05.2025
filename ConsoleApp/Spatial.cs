using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Spatial
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            Transactions.Run(config, false);
            config.LogTo(Console.WriteLine);

            using var context = new Context(config.Options);
            var order = context.Set<Order>().Skip(2).First();

            var point = new Point(51, 19) { SRID = 4326 };

            var distance = point.Distance(order.DeliveryPoint); //dystans w stopniach
            //0.424264068711927° × 111320 m/° ≈ 47236.508 metrów

            var polygon = new Polygon(new LinearRing(new Coordinate[] { new Coordinate(51, 19),
                                                                            new Coordinate(52, 20),
                                                                            new Coordinate(51, 21),
                                                                            new Coordinate(50, 20),
                                                                            new Coordinate(51, 19)}))
            { SRID = 4326};

            var intersects = polygon.Intersects(order.DeliveryPoint);
            intersects = polygon.Intersects(point);

            var orders = context.Set<Order>()
                .Where(o => o.DeliveryPoint.IsWithinDistance(point, 40000)) //dystans obliczany przez SQL w metrach
                //.Select(x => x.DeliveryPoint.Distance(point))
                .ToList();

        }
    }
}
