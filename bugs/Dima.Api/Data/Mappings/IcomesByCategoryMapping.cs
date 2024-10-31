using Dima.Core.Models.Reports;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Data.Mappings
{
    public class IcomesByCategoryMapping : IEntityTypeConfiguration<IncomesByCategory>
    {
        public void Configure(EntityTypeBuilder<IncomesByCategory> builder)
        {
            builder.ToView("vwGetIncomesByCategory");
            builder.HasNoKey();
            builder
                .Property(x => x.Incomes)
                .HasPrecision(18, 2);

            builder.Property(x => x.UserId);
            builder.Property(x => x.Year);
            builder.Property(x => x.Category);
        }
    }

}
