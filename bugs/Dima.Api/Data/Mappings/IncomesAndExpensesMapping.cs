using Dima.Core.Models.Reports;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Data.Mappings
{
    public class IncomesAndExpensesMapping : IEntityTypeConfiguration<IncomesAndExpenses>
    {
        public void Configure(EntityTypeBuilder<IncomesAndExpenses> builder)
        {
            builder.ToView("vwGetIncomesAndExpenses");
            builder.HasNoKey();
            builder
                .Property(x => x.Incomes)
                .HasPrecision(18, 2);

            builder
               .Property(x => x.Expenses)
               .HasPrecision(18, 2);

            builder.Property(x => x.UserId);
            builder.Property(x => x.Month);
            builder.Property(x => x.Year);
        }
    }
}
