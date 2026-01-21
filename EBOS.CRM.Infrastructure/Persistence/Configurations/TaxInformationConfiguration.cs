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
        builder.HasKey(ti => ti.Id);
        builder.Property(ti => ti.Id)
            .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(ti => ti.TaxName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(ti => ti.TaxIdentificationNumber)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(ti => ti.Erased)
            .IsRequired();
        
        builder.ToTable("TaxInformation", "CRM", ti =>
        {
            ti.HasCheckConstraint(
                "CK_TaxInformation_TIN_Valid",
                "[TaxIdentificationNumber] NOT LIKE '%[^A-Za-z0-9]%'"
            );
        });
        
        // ------------------------------------------------------------
        // One-to-One: TaxInformation (principal) → Customer (dependent)
        // FK: Customer.TaxInformationId
        //
        // Customer owns the FK, so the relationship is configured
        // primarily in CustomerConfiguration.
        // ------------------------------------------------------------
        builder.HasOne(ti => ti.Customer)
            .WithOne(c => c.TaxInformation)
            .HasForeignKey<Customer>(c => c.TaxInformationId)
            .OnDelete(DeleteBehavior.SetNull);
    
        // ------------------------------------------------------------
        // One-to-One: Address (principal) → TaxInformation (dependent)
        // FK: TaxInformation.AddressId
        //
        // Customer owns the FK, so the relationship is configured
        // primarily in CustomerConfiguration.
        // ------------------------------------------------------------
        builder.HasOne(ti => ti.Address) 
            .WithMany() 
            .HasForeignKey(ti => ti.AddressId) 
            .OnDelete(DeleteBehavior.Restrict); 
        
        builder.HasIndex(ti => ti.AddressId) 
            .HasDatabaseName("IX_TaxInformation_AddressId");
    }
}