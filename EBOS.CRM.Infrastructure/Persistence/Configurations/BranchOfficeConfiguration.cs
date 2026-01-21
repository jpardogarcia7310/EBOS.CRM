using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class BranchOfficeConfiguration : IEntityTypeConfiguration<BranchOffice>
{
    public void Configure(EntityTypeBuilder<BranchOffice> builder)
    {
        builder.ToTable("BranchOffices", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(bo => bo.Id);
        builder.Property(bo => bo.Id)
            .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(bo => bo.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(bo => bo.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(bo => bo.Erased)
            .IsRequired();

        // ------------------------------------------------------------
        // One-to-Many: CorporateCustomer (principal) → BranchOffice (dependent)
        // FK: BranchOffice.CorporateCustomerId
        // ------------------------------------------------------------
        builder.HasOne(bo => bo.CorporateCustomer)
            .WithMany(cc => cc.BranchOffices)
            .HasForeignKey(bo => bo.CorporateCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for FK: BranchOffices.CorporateCustomerId
        builder.HasIndex(bo => bo.CorporateCustomerId)
            .HasDatabaseName("IX_BranchOffice_CorporateCustomerId");

        // ------------------------------------------------------------
        // One-to-Many: Address (principal) → BranchOffice (dependent)
        // FK: BranchOffice.AddressId
        // ------------------------------------------------------------
        builder.HasOne(bo => bo.Address) 
            .WithOne() 
            .HasForeignKey<BranchOffice>(bo => bo.AddressId) 
            .OnDelete(DeleteBehavior.Restrict); 
        
        builder.HasIndex(d => d.AddressId) 
            .HasDatabaseName("IX_BranchOffice_AddressId"); 
    }
}