using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class IndividualCustomerConfiguration : IEntityTypeConfiguration<IndividualCustomer>
{
    public void Configure(EntityTypeBuilder<IndividualCustomer> builder)
    {
        // This entity participates in TPH inheritance, so no table mapping here.
        // Table is defined in ClienteConfiguration.

        // Basic properties
        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.IdentityDocument)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(p => p.BirthDate)
            .IsRequired();
        builder.Property(c => c.Erased)
            .IsRequired();

        // No additional relationships here.
        // All relationships are inherited from Cliente.
    }
}