using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
{
    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.ToTable("ShippingAddresses");
        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.Id).ValueGeneratedOnAdd();
        builder.Property(sa => sa.TradeName).HasMaxLength(200);

        builder.HasQueryFilter(sa => !sa.Erased);

        // Propiedades y constraints
        builder.HasOne(sa => sa.Customer)
               .WithOne(c => c.ShippingAddress)
               .HasForeignKey<ShippingAddress>(sa => sa.CustomerId);
        builder.HasOne(sa => sa.Country)
           .WithMany(co => co.ShippingAddresses)
           .HasForeignKey(sa => sa.CountryId);
    }
}