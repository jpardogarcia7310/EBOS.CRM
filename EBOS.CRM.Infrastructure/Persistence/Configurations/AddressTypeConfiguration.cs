using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class AddressTypeConfiguration : IEntityTypeConfiguration<AddressType>
{
    public void Configure(EntityTypeBuilder<AddressType> builder)
    {
        builder.ToTable("AddressTypes", "EBOS");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(t => t.Erased)
            .IsRequired();
        
        builder.ToTable("AddressTypes", "CRM", table =>
        {
            table.HasCheckConstraint( "CK_AddressType_Code_NotEmpty", "LEN([Code]) > 0" );
        }); 
        
        builder.HasIndex(at => at.Code) 
            .IsUnique() 
            .HasDatabaseName("IX_AddressType_Code_Unique");
        
        // ------------------------------------------------------------
        // One-to-N: Address (principal) → AddressType (dependent)
        // FK: Address.AddressTypeId
        // ------------------------------------------------------------
        builder.HasMany(at => at.Addresses) 
            .WithOne(a => a.AddressType) 
            .HasForeignKey(a => a.AddressTypeId) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}
