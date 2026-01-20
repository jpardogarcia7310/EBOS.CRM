using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("Statuses", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(100);

        // ------------------------------------------------------------
        // One-to-Many: Estado (principal) → Cliente (dependent)
        // FK: Cliente.EstadoId
        //
        // The relationship is configured in ClienteConfiguration.
        // ------------------------------------------------------------

        // No index here because the FK belongs to Cliente.
        // The index is created in ClienteConfiguration.
    }
}