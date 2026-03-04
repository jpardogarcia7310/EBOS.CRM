using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.EBOS;

public class AuditOutboxMessageConfiguration : IEntityTypeConfiguration<AuditOutboxMessage>
{
    public void Configure(EntityTypeBuilder<AuditOutboxMessage> builder)
    {
        builder.ToTable("AuditOutboxMessages", "EBOS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Operation).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Payload).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.NextAttemptAt).IsRequired();
        builder.Property(x => x.AttemptCount).IsRequired();
        builder.Property(x => x.LastError).HasMaxLength(2000);
        builder.Property(x => x.ProcessedAt);

        builder.HasIndex(x => new { x.ProcessedAt, x.NextAttemptAt })
            .HasDatabaseName("IX_AuditOutbox_Pending");
    }
}
