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
        builder.Property(bo => bo.AddressLine)
            .IsRequired()
            .HasMaxLength(300);
        builder.Property(bo => bo.City)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(bo => bo.PostalCode)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(bo => bo.PhoneNumber)
            .HasMaxLength(20);
        builder.Property(bo => bo.Erased)
            .IsRequired();

        // ------------------------------------------------------------
        // One-to-Many: CorporateCustomer (principal) → BranchOffice (dependent)
        // FK: BranchOffice.EmpresaId
        // ------------------------------------------------------------
        builder.HasOne(bo => bo.CorporateCustomer)
            .WithMany(cc => cc.BranchOffices)
            .HasForeignKey(bo => bo.CorporateCustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for FK: Delegacion.EmpresaId
        builder.HasIndex(bo => bo.CorporateCustomerId)
            .HasDatabaseName("IX_BranchOffice_CorporateCustomerId");

        // ------------------------------------------------------------
        // One-to-Many: Country (principal) → BranchOffice (dependent)
        // FK: BranchOffice.CountryId
        // ------------------------------------------------------------
        builder.HasOne(bo => bo.Country)
            .WithMany()
            .HasForeignKey(bo => bo.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(bo => bo.CountryId)
            .HasDatabaseName("IX_BranchOffice_CountryId");
    }
}