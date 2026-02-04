using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", "IAM");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(120);
        builder.Property(p => p.Description)
            .HasMaxLength(250);
        builder.Property(p => p.IsSystem)
            .IsRequired();
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.Erased)
            .IsRequired();

        builder.HasIndex(p => p.Code)
            .IsUnique()
            .HasDatabaseName("UX_Permission_Code");
    }
}
