using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class TaxInformationConfiguration : IEntityTypeConfiguration<TaxInformation>
{
    public void Configure(EntityTypeBuilder<TaxInformation> builder)
    {
        builder.ToTable("TaxInformation", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(df => df.Id);
        builder.Property(df => df.Id)
            .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(df => df.TaxName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(df => df.TaxIdentificationNumber)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(df => df.FiscalAddress)
            .IsRequired()
            .HasMaxLength(300);
        builder.Property(df => df.City)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(df => df.PostalCode)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(df => df.Country)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(c => c.Erased)
            .IsRequired();

        // ------------------------------------------------------------
        // One-to-One: DatosFiscales (principal) → Cliente (dependent)
        // FK: Cliente.DatosFiscalesId
        //
        // Cliente owns the FK, so the relationship is configured
        // primarily in ClienteConfiguration.
        // ------------------------------------------------------------
        builder.HasOne(df => df.Customer)
            .WithOne(c => c.TaxInformation)
            .HasForeignKey<Customer>(c => c.TaxInformationId)
            .OnDelete(DeleteBehavior.SetNull);

        // No index here because the FK belongs to Cliente.
        // The index is created in ClienteConfiguration.
    }
}