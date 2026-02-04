using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class PolicyRuleConfiguration : IEntityTypeConfiguration<PolicyRule>
{
    public void Configure(EntityTypeBuilder<PolicyRule> builder)
    {
        builder.ToTable("PolicyRules", "IAM");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(r => r.Effect)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(r => r.Priority)
            .IsRequired();
        builder.Property(r => r.IsActive)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(r => r.CreatedBy)
            .IsRequired();
        builder.Property(r => r.UpdatedAt);
        builder.Property(r => r.UpdatedBy);
        builder.Property(r => r.Erased)
            .IsRequired();

        builder.HasOne(r => r.Policy)
            .WithMany(p => p.PolicyRules)
            .HasForeignKey(r => r.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.PolicyId, r.Priority })
            .HasDatabaseName("IX_PolicyRule_Policy_Priority");
    }
}
