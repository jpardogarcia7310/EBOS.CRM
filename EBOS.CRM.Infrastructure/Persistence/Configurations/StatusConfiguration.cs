using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("Statuses", "EBOS");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(100);
    }
}
