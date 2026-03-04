using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CaseActivityConfiguration : IEntityTypeConfiguration<CaseActivity>
{
    public void Configure(EntityTypeBuilder<CaseActivity> builder)
    {
        builder.ToTable("CaseActivities", "CRM");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(a => a.Description)
            .HasMaxLength(2000);
        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(a => a.CreatedBy)
            .IsRequired();
        builder.Property(a => a.UpdatedAt);
        builder.Property(a => a.UpdatedBy);
        builder.Property(a => a.Erased)
            .IsRequired();

        builder.HasOne(a => a.Case)
            .WithMany(c => c.Activities)
            .HasForeignKey(a => a.CaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.TenantId)
            .HasDatabaseName("IX_CaseActivity_TenantId");
        builder.HasIndex(a => new { a.CaseId, a.Status })
            .HasDatabaseName("IX_CaseActivity_CaseId_Status");
    }
}
