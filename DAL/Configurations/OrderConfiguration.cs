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
        }
    }
}
