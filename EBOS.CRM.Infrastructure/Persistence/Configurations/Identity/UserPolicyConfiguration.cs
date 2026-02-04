using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class UserPolicyConfiguration : IEntityTypeConfiguration<UserPolicy>
{
    public void Configure(EntityTypeBuilder<UserPolicy> builder)
    {
        builder.ToTable("UserPolicies", "IAM");

        builder.HasKey(up => up.Id);
        builder.Property(up => up.Id).ValueGeneratedOnAdd();

        builder.Property(up => up.AssignedAt)
            .IsRequired();
        builder.Property(up => up.Erased)
            .IsRequired();

        builder.HasIndex(up => new { up.UserId, up.PolicyId })
            .IsUnique()
            .HasDatabaseName("UX_UserPolicy_User_Policy");

        builder.HasOne(up => up.User)
            .WithMany(u => u.UserPolicies)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(up => up.Policy)
            .WithMany(p => p.UserPolicies)
            .HasForeignKey(up => up.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
