using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.EBOS;

public class TenantConfigurationConfiguration : IEntityTypeConfiguration<TenantConfiguration>
{
    public void Configure(EntityTypeBuilder<TenantConfiguration> builder)
    {
        builder.ToTable("TenantConfigurations", "EBOS");

        builder.HasKey(tc => tc.Id);
        builder.Property(tc => tc.Id).ValueGeneratedOnAdd();

        builder.Property(tc => tc.TenantId).IsRequired();
        builder.Property(tc => tc.Key)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(tc => tc.ValueJson)
            .IsRequired()
            .HasMaxLength(4000);
        builder.Property(tc => tc.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(tc => tc.UpdatedBy)
            .IsRequired();
        builder.Property(tc => tc.Erased)
            .IsRequired();

        builder.HasIndex(tc => tc.TenantId)
            .HasDatabaseName("IX_TenantConfiguration_TenantId");
        builder.HasIndex(tc => new { tc.TenantId, tc.Key })
            .IsUnique()
            .HasDatabaseName("UX_TenantConfiguration_TenantId_Key");
    }
}
