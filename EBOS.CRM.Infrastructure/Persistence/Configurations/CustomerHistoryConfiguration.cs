using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CustomerHistoryConfiguration : IEntityTypeConfiguration<CustomerHistory>
{
    public void Configure(EntityTypeBuilder<CustomerHistory> builder)
    {
        builder.ToTable("CustomerHistory");
        builder.HasKey(ch => ch.Id);

        builder.Property(ch => ch.Id).ValueGeneratedOnAdd();

        // Propiedades y constraints
        builder.HasOne(ch => ch.Customer)
               .WithOne(c => c.CustomerHistory)
               .HasForeignKey<CustomerHistory>(ch => ch.CustomerId);
    }
}