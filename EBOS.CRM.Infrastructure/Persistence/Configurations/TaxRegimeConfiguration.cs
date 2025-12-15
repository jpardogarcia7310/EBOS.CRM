using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class TaxRegimeConfiguration : IEntityTypeConfiguration<TaxRegime>
{
    public void Configure(EntityTypeBuilder<TaxRegime> builder)
    {
        builder.ToTable("TasRegimes");
        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Id).ValueGeneratedOnAdd();
        builder.Property(tr => tr.Description).IsRequired().HasMaxLength(100);
    }
}