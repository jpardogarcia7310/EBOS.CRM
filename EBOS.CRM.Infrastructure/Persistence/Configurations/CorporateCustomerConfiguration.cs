using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CorporateCustomerConfiguration : IEntityTypeConfiguration<CorporateCustomer>
{
    public void Configure(EntityTypeBuilder<CorporateCustomer> builder)
    {
        // This entity participates in TPH inheritance, so no table mapping here.
        // Table is defined in ClienteConfiguration.

        // Basic properties
        builder.Property(e => e.LegalName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(e => e.TaxIdentification)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(c => c.Erased)
            .IsRequired();

        // ------------------------------------------------------------
        // One-to-Many: Empresa (principal) → Delegacion (dependent)
        // FK: Delegacion.EmpresaId
        // ------------------------------------------------------------
        builder.HasMany(e => e.BranchOffices)
            .WithOne(d => d.CorporateCustomer)
            .HasForeignKey(d => d.CorporateCustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for FK: Delegacion.EmpresaId
        // (Created in DelegacionConfiguration, because FK belongs to Delegacion)
    }
}