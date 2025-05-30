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
    internal class Json
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            config.LogTo(Console.WriteLine);

            using var context = new Context(config.Options);

            var person = new Person() { FirstName = "Ewa", Address = new Address() { City = "Katowice", Street = "Krakowska", Number = 19, Coordinates = new Models.Coordinates { Longitude = 50, Latitude = 19 } } };
        
            context.Add(person);

            person = new Person() { FirstName = "Adam", Address = new Address() { City = "Kraków", Street = "Katowicka", Number = 91, Coordinates = new Models.Coordinates { Longitude = 51, Latitude = 20 } } };

            context.Add(person);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            person = context.Set<Person>().FirstOrDefault(x => x.Address.Number == 19);
            person.Address.Number = 0;

            context.SaveChanges();
        }
    }
}
