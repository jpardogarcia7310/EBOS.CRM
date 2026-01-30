using EBOS.CRM.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class TaxInformationAddressConfiguration : IEntityTypeConfiguration<TaxInformationAddress>
{
    public void Configure(EntityTypeBuilder<TaxInformationAddress> builder)
    {
        builder.ToTable("TaxInformationAddresses", "CRM");

        builder.HasKey(ta => ta.Id);
        builder.Property(ta => ta.Id).ValueGeneratedOnAdd();

        builder.Property(ta => ta.IsPrimary)
            .IsRequired();
        builder.Property(ta => ta.ValidFrom)
            .IsRequired();
        builder.Property(ta => ta.IsCurrent)
            .IsRequired();

        builder.HasOne(ta => ta.TaxInformation)
            .WithMany(ti => ti.TaxInformationAddresses)
            .HasForeignKey(ta => ta.TaxInformationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ta => ta.Address)
            .WithMany(a => a.TaxInformationAddresses)
            .HasForeignKey(ta => ta.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ta => new { ta.TaxInformationId, ta.IsCurrent, ta.IsPrimary })
            .HasDatabaseName("IX_TaxInformationAddress_Current_Primary");
    }
}