using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "IAM");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.ExternalId)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(120);
        builder.Property(u => u.IsActive)
            .IsRequired();
        builder.Property(u => u.CreatedAt)
            .IsRequired();
        builder.Property(u => u.Erased)
            .IsRequired();

        builder.HasIndex(u => u.ExternalId)
            .IsUnique()
            .HasDatabaseName("UX_User_ExternalId");
        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("UX_User_Username");
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("UX_User_Email");
    }
}
