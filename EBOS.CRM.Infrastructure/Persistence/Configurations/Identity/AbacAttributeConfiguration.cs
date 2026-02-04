using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.Identity;

public class AbacAttributeConfiguration : IEntityTypeConfiguration<AbacAttribute>
{
    public void Configure(EntityTypeBuilder<AbacAttribute> builder)
    {
        builder.ToTable("AbacAttributes", "IAM");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.Code)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(a => a.Category)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(a => a.DataType)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(a => a.Description)
            .HasMaxLength(250);
        builder.Property(a => a.IsActive)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(a => a.CreatedBy)
            .IsRequired();
        builder.Property(a => a.UpdatedAt);
        builder.Property(a => a.UpdatedBy);
        builder.Property(a => a.Erased)
            .IsRequired();

        builder.HasIndex(a => a.Code)
            .IsUnique()
            .HasDatabaseName("UX_AbacAttribute_Code");
        builder.HasIndex(a => new { a.Category, a.Code })
            .HasDatabaseName("IX_AbacAttribute_Category_Code");
    }
}
