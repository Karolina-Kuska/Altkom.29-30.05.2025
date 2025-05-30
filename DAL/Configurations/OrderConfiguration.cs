using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

            builder.Property(x => x.TotalValue).HasComputedColumnSql("[Value] * (1 + [Tax])", stored: true);

            builder.Property<DateTime>("CurrentDate").HasComputedColumnSql("GETDATE()");
            //builder.Property(x => x.IsExpired).HasComputedColumnSql("CASE WHEN [OrderDate] < GETDATE() THEN 1 ELSE 0 END");

            /*builder.Property(x => x.OrderType).HasConversion(
                x => x.ToString(),
                x => Enum.Parse<OrderType>(x));*/
            //builder.Property(x => x.OrderType).HasConversion(new EnumToStringConverter<OrderType>());
            builder.Property(x => x.OrderType).HasConversion<string>();
            builder.Property(x => x.Parameters).HasConversion<string>();
        }
    }
}
