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
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(d => d.AddressLine)
            .IsRequired()
            .HasMaxLength(300);
        builder.Property(d => d.City)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(d => d.PostalCode)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(d => d.Country)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(d => d.PhoneNumber)
            .HasMaxLength(20);
        builder.Property(c => c.Erased)
            .IsRequired();

        // ------------------------------------------------------------
        // One-to-Many: Empresa (principal) → Delegacion (dependent)
        // FK: Delegacion.EmpresaId
        // ------------------------------------------------------------
        builder.HasOne(d => d.CorporateCustomer)
            .WithMany(e => e.BranchOffices)
            .HasForeignKey(d => d.CorporateCustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for FK: Delegacion.EmpresaId
        builder.HasIndex(d => d.CorporateCustomerId)
            .HasDatabaseName("IX_BranchOffice_CorporateCustomerId");
    }
}