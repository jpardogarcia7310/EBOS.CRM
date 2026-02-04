using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions", "IAM");

        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Id).ValueGeneratedOnAdd();

        builder.Property(rp => rp.AssignedAt)
            .IsRequired();
        builder.Property(rp => rp.Erased)
            .IsRequired();

        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique()
            .HasDatabaseName("UX_RolePermission_Role_Permission");

        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
