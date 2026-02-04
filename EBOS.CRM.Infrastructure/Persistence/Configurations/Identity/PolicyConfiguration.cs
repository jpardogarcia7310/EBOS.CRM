using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies", "IAM");

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
        builder.Property(p => p.IsActive)
            .IsRequired();
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.Erased)
            .IsRequired();

        builder.HasIndex(p => p.Code)
            .IsUnique()
            .HasDatabaseName("UX_Policy_Code");
    }
}
