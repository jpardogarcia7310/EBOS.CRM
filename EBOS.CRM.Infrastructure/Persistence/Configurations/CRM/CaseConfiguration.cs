using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases", "CRM");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(c => c.Description)
            .HasMaxLength(2000);
        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.Priority)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.OwnerUserId)
            .IsRequired();
        builder.Property(c => c.QueueId)
            .IsRequired();
        builder.Property(c => c.SlaId)
            .IsRequired();
        builder.Property(c => c.DueAt);
        builder.Property(c => c.ClosedAt);
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(c => c.CreatedBy)
            .IsRequired();
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.UpdatedBy);
        builder.Property(c => c.Erased)
            .IsRequired();

        builder.HasOne(c => c.Queue)
            .WithMany(q => q.Cases)
            .HasForeignKey(c => c.QueueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Sla)
            .WithMany(s => s.Cases)
            .HasForeignKey(c => c.SlaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.TenantId)
            .HasDatabaseName("IX_Case_TenantId");
        builder.HasIndex(c => new { c.Status, c.CreatedAt })
            .HasDatabaseName("IX_Case_Status_CreatedAt");
        builder.HasIndex(c => c.QueueId)
            .HasDatabaseName("IX_Case_QueueId");
        builder.HasIndex(c => c.SlaId)
            .HasDatabaseName("IX_Case_SlaId");
    }
}
