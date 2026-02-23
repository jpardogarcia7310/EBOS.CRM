using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.EBOS;

public class ChannelCountryConfiguration : IEntityTypeConfiguration<ChannelCountry>
{
    public void Configure(EntityTypeBuilder<ChannelCountry> builder)
    {
        builder.ToTable("ChannelCountries", "EBOS");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ChannelTypeId).IsRequired();
        builder.Property(x => x.CountryId).IsRequired();
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.UpdatedBy).IsRequired();

        builder.HasOne(x => x.ChannelType)
            .WithMany(x => x.ChannelCountries)
            .HasForeignKey(x => x.ChannelTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Country)
            .WithMany(x => x.ChannelCountries)
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ChannelTypeId, x.CountryId })
            .IsUnique()
            .HasDatabaseName("UX_ChannelCountries_ChannelTypeId_CountryId");
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_ChannelCountries_IsActive");
    }
}
