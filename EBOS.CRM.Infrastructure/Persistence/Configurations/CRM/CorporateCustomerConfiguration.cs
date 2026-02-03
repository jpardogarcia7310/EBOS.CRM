using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CorporateCustomerConfiguration : IEntityTypeConfiguration<CorporateCustomer>
{
    public void Configure(EntityTypeBuilder<CorporateCustomer> builder)
    {
        // This entity participates in TPH inheritance, so no table mapping here.
        // Table is defined in CustomerConfiguration.

        // Basic properties
        builder.Property(e => e.LegalName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(e => e.TaxIdentification)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(c => c.Erased)
            .IsRequired();

        builder.ToTable("Customers", "CRM", c =>
        {
            c.HasCheckConstraint(
                "CK_CorporateCustomer_TaxId_Valid",
                "[TaxIdentification] NOT LIKE '%[^A-Za-z0-9]%'"
            );
        });

        // ------------------------------------------------------------
        // One-to-Many: CorporateCustomer (principal) → BranchOffices (dependent)
        // FK: BranchOffices.CorporateCustomerId
        // ------------------------------------------------------------
        builder.HasMany(cc => cc.BranchOffices)
            .WithOne(bo => bo.CorporateCustomer)
            .HasForeignKey(bo => bo.CorporateCustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
