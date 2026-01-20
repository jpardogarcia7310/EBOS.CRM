using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class BankInformationConfiguration : IEntityTypeConfiguration<BankInformation>
{
    public void Configure(EntityTypeBuilder<BankInformation> builder)
    {
        builder.ToTable("BankInformation", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(db => db.Id);
        builder.Property(db => db.Id)
            .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(db => db.Iban)
            .IsRequired()
            .HasMaxLength(34); // IBAN max length
        builder.Property(db => db.Bic)
            .HasMaxLength(11); // BIC max length
        builder.Property(db => db.BankName)
            .HasMaxLength(200);
        builder.Property(c => c.Erased)
            .IsRequired();

        // ------------------------------------------------------------
        // One-to-One: BankInformation (principal) → Customer (dependent)
        // FK: Customer.BankInformationId
        //
        // Customer owns the FK, so the relationship is configured
        // primarily in CustomerConfiguration.
        // ------------------------------------------------------------
        builder.HasOne(db => db.Customer)
            .WithOne(c => c.BankInformation)
            .HasForeignKey<Customer>(c => c.BankInformationId)
            .OnDelete(DeleteBehavior.SetNull);

        // No index here because the FK belongs to Customer.
        // The index is created in CustomerConfiguration.
    }
}