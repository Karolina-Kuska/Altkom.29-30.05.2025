using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DAL.Conventions
{
    internal class PluralizeTableNameConvention : IModelFinalizingConvention
    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            modelBuilder.Metadata.GetEntityTypes()
                ////GetTableName() zwraca aktualną nazwę tabeli
                //GetDefaultTableName() zwraca pierwotną nazwę tabeli 
                //umożliwiamy zmianę nazwy tabeli w konfiguracji encji
                .Where(x => x.GetDefaultTableName() == x.GetTableName())
                .ToList()
                //domyślne nazwy tablek pluralizujemy
                .ForEach(x => x.SetTableName(new Pluralize.NET.Core.Pluralizer().Pluralize(x.GetDefaultTableName())));
        }
    }
}
