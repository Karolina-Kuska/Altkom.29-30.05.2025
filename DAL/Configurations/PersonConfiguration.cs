using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DAL.Configurations
{
    internal class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            //builder.HasKey(p => p.Key);

            builder.ToTable(x => x.IsTemporal(x =>
            {
                x.HasPeriodEnd("To"); //Domyślnie: "PeriodEnd"
                x.HasPeriodStart("From"); //Domyślnie: "PeriodStart"
                x.UseHistoryTable("PeopleHistory"); //Domyślnie: "PersonHistory"
            }));

            builder.Property(x => x.OptionalDescription).IsSparse();
        }
    }
}
