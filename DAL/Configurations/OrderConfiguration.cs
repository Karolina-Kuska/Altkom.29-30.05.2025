using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configurations
{
    internal class OrderConfiguration : EntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            //concurrency token - zabezpiecza przed konfliktami tylko wskazane właściwości
            builder.Property(x => x.OrderDate).IsConcurrencyToken();

            //informujemy EF Core, że właściwość Name ma backing field o nazwie "orderName", czyli inny niż domyślnie generowany przez konwencje EF Core
            builder.Property(x => x.Name).HasField("orderName");
        }
    }
}
