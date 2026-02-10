using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        builder.ToTable("Opportunities", "CRM");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedOnAdd();

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(o => o.OwnerUserId)
            .IsRequired();
        builder.Property(o => o.Amount)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(o => o.Probability)
            .IsRequired()
            .HasPrecision(5, 4);
        builder.Property(o => o.Source)
            .HasMaxLength(100);
        builder.Property(o => o.CloseReason)
            .HasMaxLength(500);
        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(o => o.CreatedBy)
            .IsRequired();
        builder.Property(o => o.UpdatedAt);
        builder.Property(o => o.UpdatedBy);
        builder.Property(o => o.Erased)
            .IsRequired();

        builder.ToTable("Opportunities", "CRM", o =>
        {
            o.HasCheckConstraint(
                "CK_Opportunity_Amount_NonNegative",
                "[Amount] >= 0");
            o.HasCheckConstraint(
                "CK_Opportunity_Probability_Range",
                "[Probability] >= 0 AND [Probability] <= 1");
        });

        builder.HasOne(o => o.Stage)
            .WithMany(s => s.Opportunities)
            .HasForeignKey(o => o.StageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.SourceLead)
            .WithOne(l => l.ConvertedOpportunity)
            .HasForeignKey<Opportunity>(o => o.SourceLeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Quotes)
            .WithOne(q => q.Opportunity)
            .HasForeignKey(q => q.OpportunityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.TenantId)
            .HasDatabaseName("IX_Opportunity_TenantId");
        builder.HasIndex(o => new { o.StageId, o.ExpectedCloseDate })
            .HasDatabaseName("IX_Opportunity_Stage_CloseDate");
        builder.HasIndex(o => new { o.OwnerUserId, o.StageId })
            .HasDatabaseName("IX_Opportunity_Owner_Stage");
    }
}
