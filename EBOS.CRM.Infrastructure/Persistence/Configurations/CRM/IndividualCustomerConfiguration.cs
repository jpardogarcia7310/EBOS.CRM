using EBOS.CRM.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class IndividualCustomerConfiguration : IEntityTypeConfiguration<IndividualCustomer>
{
    public void Configure(EntityTypeBuilder<IndividualCustomer> builder)
    {
        // This entity participates in TPH inheritance, so no table mapping here.
        // Table is defined in CustomerConfiguration.

        // Basic properties
        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(c => c.IdentificationNumber)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(c => c.BirthDate)
            .IsRequired();
        builder.Property(c => c.Erased)
            .IsRequired();

        // ------------------------------------------------------------
        // One-to-One: IndividualCustomer (principal) → IdentificationType (dependent)
        // FK: IndividualCustomer.IdentificationTypeId
        // ------------------------------------------------------------
        builder.HasOne(c => c.IdentificationType)
            .WithMany()
            .HasForeignKey(c => c.IdentificationTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        // Index for FK: IndividualCustomer.IdentificationTypeId
        builder.HasIndex(c => c.IdentificationTypeId)
            .HasDatabaseName("IX_IndividualCustomer_IdentificationTypeId");
    }
}