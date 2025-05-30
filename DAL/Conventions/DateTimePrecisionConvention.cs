using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DAL.Conventions
{
    internal class DateTimePrecisionConvention : IModelFinalizingConvention
    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var property in modelBuilder.Metadata.GetEntityTypes().SelectMany(x => x.GetProperties()).Where(x => x.ClrType == typeof(DateTime)))
                property.SetPrecision(5);
        }
    }
}
