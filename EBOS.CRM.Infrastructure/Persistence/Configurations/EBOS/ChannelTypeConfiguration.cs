using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.EBOS;

public class ChannelTypeConfiguration : IEntityTypeConfiguration<ChannelType>
{
    public void Configure(EntityTypeBuilder<ChannelType> builder)
    {
        builder.ToTable("ChannelTypes", "EBOS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.UpdatedBy).IsRequired();

        builder.HasIndex(x => x.Descripcion)
            .HasDatabaseName("IX_ChannelTypes_Descripcion");
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_ChannelTypes_IsActive");
    }
}
