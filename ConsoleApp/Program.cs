using Microsoft.EntityFrameworkCore;
using DAL;
using ConsoleApp;
using Models;

var config = new DbContextOptionsBuilder<Context>()
    .UseSqlServer("Server=(local);Database=ef;Integrated Security=true;TrustServerCertificate=True;");

using ( var context = new Context(config.Options))
{
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

//ChangeTracker.Run(config.Options);
//ChangeTracker.TrackingProxies(config);
//ChangeTracker.ChangedNotification(config.Options);

//ConcurrencyCheck.Run(config);

//ShadowProperty.Run(config);

//GlobalFilters.Run(config);

//Transactions.Run(config);

//RelatedData.Run(config);

//TemporalTable.Run(config);

//CompiledQuery.Run(config);

Transactions.Run(config, false);
using (var context = new Context(config.Options))
{
    var order = new Order { Name = "Test Order", Value = 123, Tax = 0.23f };
    context.Add(order);
    context.SaveChanges();

    context.ChangeTracker.Clear();
}
using (var context = new Context(config.Options))
{
    var order = context.Set<Order>().FirstOrDefault(o => o.Name == "Test Order");

    Console.WriteLine($"Order Name: {order?.Name}, Value: {order?.Value}, Tax: {order?.Tax}, TotalValue: {order?.TotalValue}");
}

config.LogTo(Console.WriteLine);
using (var context = new Context(config.Options))
{
    //Tabele dzielone pozwalają nam nie tylko poprawić organizację klas, ale także pobierać dane partiami z jednej tabeli, co jest przydatne w przypadku dużych zbiorów danych.
    var products = context.Set<Product>().Take(2).ToList();
    context.Entry(products[0]).Reference(x => x.ProductDetails).Load();
    products = context.Set<Product>().Include(x => x.ProductDetails).ToList();

    //alternatywą jest użycie Select, aby pobrać tylko wybrane właściwości
    products =  context.Set<Product>().Select(x => new Product { Name = x.Name, Price = x.Price }).ToList();
}