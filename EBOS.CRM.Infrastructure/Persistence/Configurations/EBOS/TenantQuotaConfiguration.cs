using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.EBOS;

public class TenantQuotaConfiguration : IEntityTypeConfiguration<TenantQuota>
{
    public void Configure(EntityTypeBuilder<TenantQuota> builder)
    {
        builder.ToTable("TenantQuotas", "EBOS");

        builder.HasKey(tq => tq.Id);
        builder.Property(tq => tq.Id).ValueGeneratedOnAdd();

        builder.Property(tq => tq.TenantId).IsRequired();
        builder.Property(tq => tq.Metric)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(tq => tq.Limit)
            .IsRequired()
            .HasPrecision(18, 4);
        builder.Property(tq => tq.Unit)
            .HasMaxLength(20);
        builder.Property(tq => tq.EffectiveFrom)
            .IsRequired();
        builder.Property(tq => tq.EffectiveTo);
        builder.Property(tq => tq.Erased)
            .IsRequired();

        builder.HasIndex(tq => tq.TenantId)
            .HasDatabaseName("IX_TenantQuota_TenantId");
        builder.HasIndex(tq => new { tq.TenantId, tq.Metric, tq.EffectiveFrom })
            .IsUnique()
            .HasDatabaseName("UX_TenantQuota_TenantId_Metric_EffectiveFrom");
    }
}
