using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "IAM");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.Code)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(r => r.Description)
            .HasMaxLength(250);
        builder.Property(r => r.IsSystem)
            .IsRequired();
        builder.Property(r => r.IsActive)
            .IsRequired();
        builder.Property(r => r.CreatedAt)
            .IsRequired();
        builder.Property(r => r.Erased)
            .IsRequired();

        builder.HasIndex(r => r.Code)
            .IsUnique()
            .HasDatabaseName("UX_Role_Code");
    }
}
