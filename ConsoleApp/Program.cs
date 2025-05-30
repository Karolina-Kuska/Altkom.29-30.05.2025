using Microsoft.EntityFrameworkCore;
using DAL;
using ConsoleApp;
using Models;

var config = new DbContextOptionsBuilder<Context>()
    .UseSqlServer("Server=(local);Database=ef;Integrated Security=true;TrustServerCertificate=True;");

using ( var context = new Context(config.Options))
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
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

CompiledQuery.Run(config);