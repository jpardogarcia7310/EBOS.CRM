using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.EBOS;

public class AddressTypeConfiguration : IEntityTypeConfiguration<AddressType>
{
    public void Configure(EntityTypeBuilder<AddressType> builder)
    {
        builder.ToTable("AddressTypes", "EBOS");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(t => t.CreatedBy)
            .IsRequired();
        builder.Property(t => t.UpdatedAt);
        builder.Property(t => t.UpdatedBy);

        builder.Property(t => t.Category)
            .HasMaxLength(50);
        builder.Property(t => t.AllowsMultiple)
            .IsRequired()
            .HasDefaultValue(true);
        builder.Property(t => t.RequiresPrimary)
            .IsRequired()
            .HasDefaultValue(false);
    }
}

