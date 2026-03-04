using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class PolicyRoleConfiguration : IEntityTypeConfiguration<PolicyRole>
{
    public void Configure(EntityTypeBuilder<PolicyRole> builder)
    {
        builder.ToTable("PolicyRoles", "IAM");

        builder.HasKey(pr => pr.Id);
        builder.Property(pr => pr.Id).ValueGeneratedOnAdd();

        builder.Property(pr => pr.AssignedAt)
            .IsRequired();
        builder.Property(pr => pr.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(pr => pr.CreatedBy)
            .IsRequired();
        builder.Property(pr => pr.UpdatedAt);
        builder.Property(pr => pr.UpdatedBy);
        builder.Property(pr => pr.Erased)
            .IsRequired();

        builder.HasIndex(pr => new { pr.PolicyId, pr.RoleId })
            .IsUnique()
            .HasDatabaseName("UX_PolicyRole_Policy_Role");

        builder.HasOne(pr => pr.Policy)
            .WithMany(p => p.PolicyRoles)
            .HasForeignKey(pr => pr.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pr => pr.Role)
            .WithMany(r => r.PolicyRoles)
            .HasForeignKey(pr => pr.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
