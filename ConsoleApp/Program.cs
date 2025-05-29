using Microsoft.EntityFrameworkCore;
using DAL;

var config = new DbContextOptionsBuilder<Context>()
    .UseSqlServer("Server=(local);Database=ef;Integrated Security=true;TrustServerCertificate=True;")
    .Options;

using ( var context = new Context(config))
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}