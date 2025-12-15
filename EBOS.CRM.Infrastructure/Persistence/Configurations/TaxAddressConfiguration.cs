using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class TaxAddressConfiguration : IEntityTypeConfiguration<TaxAddress>
{
    public void Configure(EntityTypeBuilder<TaxAddress> builder)
    {
        builder.ToTable("TaxAddresses");
        builder.HasKey(ta => ta.Id);

        builder.Property(ta => ta.Id).ValueGeneratedOnAdd();
        builder.Property(ta => ta.Street).HasMaxLength(200);

        builder.HasQueryFilter(ta => !ta.Erased);

        // Propiedades y constraints
        builder.HasOne(ta => ta.Customer)
            .WithOne(c => c.TaxAddress)
            .HasForeignKey<TaxAddress>(ta => ta.CustomerId);
        builder.HasOne(ta => ta.Country)
           .WithMany(co => co.TaxAddresses)
           .HasForeignKey(ta => ta.CountryId);
    }
}