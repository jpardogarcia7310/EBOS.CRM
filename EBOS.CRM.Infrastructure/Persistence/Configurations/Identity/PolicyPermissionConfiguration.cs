using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class PolicyPermissionConfiguration : IEntityTypeConfiguration<PolicyPermission>
{
    public void Configure(EntityTypeBuilder<PolicyPermission> builder)
    {
        builder.ToTable("PolicyPermissions", "IAM");

        builder.HasKey(pp => pp.Id);
        builder.Property(pp => pp.Id).ValueGeneratedOnAdd();

        builder.Property(pp => pp.AssignedAt)
            .IsRequired();
        builder.Property(pp => pp.Erased)
            .IsRequired();

        builder.HasIndex(pp => new { pp.PolicyId, pp.PermissionId })
            .IsUnique()
            .HasDatabaseName("UX_PolicyPermission_Policy_Permission");

        builder.HasOne(pp => pp.Policy)
            .WithMany(p => p.PolicyPermissions)
            .HasForeignKey(pp => pp.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pp => pp.Permission)
            .WithMany(p => p.PolicyPermissions)
            .HasForeignKey(pp => pp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
