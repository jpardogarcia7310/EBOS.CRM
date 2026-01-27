using EBOS.CRM.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

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
        // FK: BankInformation.CustomerId
        //
        // BankInformation owns the FK, so the relationship is configured
        // primarily in BankInformationConfiguration.
        // ------------------------------------------------------------
        builder.HasOne(bi => bi.Customer)
            .WithOne(c => c.BankInformation)
            .HasForeignKey<BankInformation>(bi => bi.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}