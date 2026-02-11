using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class SlaConfiguration : IEntityTypeConfiguration<Sla>
{
    public void Configure(EntityTypeBuilder<Sla> builder)
    {
        builder.ToTable("Slas", "CRM");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(s => s.TargetMinutes)
            .IsRequired();
        builder.Property(s => s.WarningMinutes);
        builder.Property(s => s.ActiveFrom);
        builder.Property(s => s.ActiveTo);
        builder.Property(s => s.IsActive)
            .IsRequired();
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(s => s.CreatedBy)
            .IsRequired();
        builder.Property(s => s.UpdatedAt);
        builder.Property(s => s.UpdatedBy);
        builder.Property(s => s.Erased)
            .IsRequired();

        builder.ToTable("Slas", "CRM", s =>
        {
            s.HasCheckConstraint(
                "CK_Sla_TargetMinutes_Positive",
                "[TargetMinutes] > 0");
            s.HasCheckConstraint(
                "CK_Sla_WarningMinutes_NonNegative",
                "[WarningMinutes] IS NULL OR [WarningMinutes] >= 0");
        });

        builder.HasIndex(s => s.TenantId)
            .HasDatabaseName("IX_Sla_TenantId");
        builder.HasIndex(s => new { s.TenantId, s.Name })
            .HasDatabaseName("UX_Sla_TenantId_Name")
            .IsUnique();
    }
}
