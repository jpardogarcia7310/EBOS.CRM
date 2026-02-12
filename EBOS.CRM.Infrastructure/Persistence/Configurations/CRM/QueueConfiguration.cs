using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class QueueConfiguration : IEntityTypeConfiguration<Queue>
{
    public void Configure(EntityTypeBuilder<Queue> builder)
    {
        builder.ToTable("Queues", "CRM");

        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).ValueGeneratedOnAdd();

        builder.Property(q => q.Name)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(q => q.Code)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(q => q.IsActive)
            .IsRequired();
        builder.Property(q => q.DefaultOwnerUserId);
        builder.Property(q => q.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(q => q.CreatedBy)
            .IsRequired();
        builder.Property(q => q.UpdatedAt);
        builder.Property(q => q.UpdatedBy);
        builder.Property(q => q.Erased)
            .IsRequired();

        builder.HasIndex(q => q.TenantId)
            .HasDatabaseName("IX_Queue_TenantId");
        builder.HasIndex(q => new { q.TenantId, q.Code })
            .HasDatabaseName("UX_Queue_TenantId_Code")
            .IsUnique();
    }
}
