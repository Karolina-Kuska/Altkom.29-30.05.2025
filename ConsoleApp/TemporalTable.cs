using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class TemporalTable
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            using var context = new Context(config.Options);

            var person = new Person             
            {
                FirstName = "Jan",
                LastName = "Kowalski",
            };

            context.Add(person);
            context.SaveChanges();

            Thread.Sleep(2500);

            person.FirstName = "Janusz";
            context.SaveChanges();


            Thread.Sleep(2500);

            person.LastName = "Nowak";
            context.SaveChanges();

            Thread.Sleep(2500);
            person.FirstName = "Janek";
            context.SaveChanges();

            context.ChangeTracker.Clear();

            person = context.Set<Person>().First();
            var people = context.Set<Person>().ToArray();

            var data = context.Set<Person>().TemporalAll()
                .Select(x => new { x, FROM = EF.Property<DateTime>(x, "From"), TO = EF.Property<DateTime>(x, "To") }).ToList();

            Console.WriteLine($"Obecny stan: {person.FirstName} {person.LastName}");
            person = context.Set<Person>().TemporalAsOf(DateTime.UtcNow.AddSeconds(-5)).Single();
            Console.WriteLine($"Stan z przed 5 sekund: {person.FirstName} {person.LastName}");

            people = context.Set<Person>().TemporalBetween(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(-1)).ToArray();
            foreach (var p in people)
            {
                Console.WriteLine($"Stan pomiędzy 5 a 1 sek w przeszłości: {p.FirstName} {p.LastName}");
            }
        }
    }
}
