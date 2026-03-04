using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.EBOS;

public class TenantUsageMetricConfiguration : IEntityTypeConfiguration<TenantUsageMetric>
{
    public void Configure(EntityTypeBuilder<TenantUsageMetric> builder)
    {
        builder.ToTable("TenantUsageMetrics", "EBOS");

        builder.HasKey(tm => tm.Id);
        builder.Property(tm => tm.Id).ValueGeneratedOnAdd();

        builder.Property(tm => tm.TenantId).IsRequired();
        builder.Property(tm => tm.Metric)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(tm => tm.Value)
            .IsRequired()
            .HasPrecision(18, 4);
        builder.Property(tm => tm.Unit)
            .HasMaxLength(20);
        builder.Property(tm => tm.PeriodStart)
            .IsRequired();
        builder.Property(tm => tm.PeriodEnd)
            .IsRequired();
        builder.Property(tm => tm.Source)
            .HasMaxLength(100);
        builder.Property(tm => tm.Erased)
            .IsRequired();

        builder.HasIndex(tm => tm.TenantId)
            .HasDatabaseName("IX_TenantUsageMetric_TenantId");
        builder.HasIndex(tm => new { tm.TenantId, tm.Metric, tm.PeriodStart })
            .HasDatabaseName("IX_TenantUsageMetric_TenantId_Metric_PeriodStart");
    }
}
