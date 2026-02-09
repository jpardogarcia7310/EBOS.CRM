using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.EBOS;

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
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(e => e.CreatedBy)
            .IsRequired();
        builder.Property(e => e.UpdatedAt);
        builder.Property(e => e.UpdatedBy);
    }
}

