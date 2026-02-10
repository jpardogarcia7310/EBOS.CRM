using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class OpportunityStageConfiguration : IEntityTypeConfiguration<OpportunityStage>
{
    public void Configure(EntityTypeBuilder<OpportunityStage> builder)
    {
        builder.ToTable("OpportunityStages", "CRM");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.Order)
            .IsRequired();
        builder.Property(s => s.DefaultProbability)
            .IsRequired()
            .HasPrecision(5, 4);
        builder.Property(s => s.IsClosed)
            .IsRequired();
        builder.Property(s => s.IsWon)
            .IsRequired();
        builder.Property(s => s.Erased)
            .IsRequired();

        builder.ToTable("OpportunityStages", "CRM", s =>
        {
            s.HasCheckConstraint(
                "CK_OpportunityStage_DefaultProbability_Range",
                "[DefaultProbability] >= 0 AND [DefaultProbability] <= 1");
        });

        builder.HasIndex(s => s.TenantId)
            .HasDatabaseName("IX_OpportunityStage_TenantId");
        builder.HasIndex(s => new { s.TenantId, s.Order })
            .HasDatabaseName("IX_OpportunityStage_TenantId_Order");
        builder.HasIndex(s => new { s.TenantId, s.Name })
            .HasDatabaseName("UX_OpportunityStage_TenantId_Name")
            .IsUnique();
    }
}
