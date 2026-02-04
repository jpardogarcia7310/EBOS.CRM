using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class PolicyRuleConditionConfiguration : IEntityTypeConfiguration<PolicyRuleCondition>
{
    public void Configure(EntityTypeBuilder<PolicyRuleCondition> builder)
    {
        builder.ToTable("PolicyRuleConditions", "IAM");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Operator)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.Value)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(c => c.ValueType)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(c => c.IsNegated)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(c => c.CreatedBy)
            .IsRequired();
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.UpdatedBy);
        builder.Property(c => c.Erased)
            .IsRequired();

        builder.HasOne(c => c.PolicyRule)
            .WithMany(r => r.Conditions)
            .HasForeignKey(c => c.PolicyRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Attribute)
            .WithMany(a => a.PolicyRuleConditions)
            .HasForeignKey(c => c.AttributeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.PolicyRuleId, c.AttributeId })
            .HasDatabaseName("IX_PolicyRuleCondition_Rule_Attribute");
    }
}
