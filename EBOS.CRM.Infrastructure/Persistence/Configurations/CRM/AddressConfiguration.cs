using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses", "CRM");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.Street)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(a => a.ExternalNumber)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(a => a.InternalNumber)
            .HasMaxLength(20);
        builder.Property(a => a.BetweenStreet1)
            .HasMaxLength(200);
        builder.Property(a => a.BetweenStreet2)
            .HasMaxLength(200);
        builder.Property(a => a.Neighbourhood)
            .HasMaxLength(200);
        builder.Property(a => a.City)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(a => a.StateOrProvince)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(a => a.PostalCode)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(a => a.GoogleMapsUrl)
            .HasMaxLength(500);
        builder.Property(a => a.Latitude)
            .HasPrecision(10, 6);
        builder.Property(a => a.Longitude)
            .HasPrecision(10, 6);
        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(a => a.CreatedBy)
            .IsRequired();
        builder.Property(a => a.UpdatedAt);
        builder.Property(a => a.UpdatedBy);
        builder.Property(a => a.Erased)
            .IsRequired();

        builder.ToTable("Addresses", "CRM", a =>
        {
            a.HasCheckConstraint(
                "CK_Address_Latitude_Range",
                "[Latitude] IS NULL OR ([Latitude] BETWEEN -90 AND 90)");
            a.HasCheckConstraint(
                "CK_Address_Longitude_Range",
                "[Longitude] IS NULL OR ([Longitude] BETWEEN -180 AND 180)");
            a.HasCheckConstraint(
                "CK_Address_PostalCode_Length",
                "LEN([PostalCode]) >= 3");
            a.HasCheckConstraint(
                "CK_Address_GoogleMapsUrl_Valid",
                "[GoogleMapsUrl] IS NULL OR [GoogleMapsUrl] LIKE 'https://maps.%'");
        });

        builder.HasOne(a => a.Country)
            .WithMany()
            .HasForeignKey(a => a.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AddressType)
            .WithMany(t => t.Addresses)
            .HasForeignKey(a => a.AddressTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.City, a.StateOrProvince })
            .HasDatabaseName("IX_Address_City_State");
        builder.HasIndex(a => new { a.CountryId, a.City })
            .HasDatabaseName("IX_Address_Country_City");
    }
}

