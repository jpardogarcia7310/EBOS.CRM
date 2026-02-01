using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries", "EBOS");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);
        builder.Property(c => c.Iso31661A2Code)
                .IsRequired()
                .HasMaxLength(2);
        builder.Property(c => c.Iso31661A3Code)
                .IsRequired()
                .HasMaxLength(3);
        builder.Property(c => c.Iso31661NumCode)
                .IsRequired()
                .HasMaxLength(10);
        builder.Property(c => c.Domain)
                .IsRequired()
                .HasMaxLength(5);
        builder.Property(c => c.Currency)
                .IsRequired()
                .HasMaxLength(100);
        builder.Property(c => c.CurrencyCode)
                .IsRequired()
                .HasMaxLength(10);
        builder.Property(c => c.InternationalPhoneCode)
                .IsRequired()
                .HasMaxLength(20);

        // Table declaration with its additional constraints (example: check format if desired)
        // Note: EF does not validate complex expressions in the database; if you want database-level constraints,
        // you can add CHECK constraints with HasCheckConstraint:
        builder.ToTable("Countries", "EBOS", c =>
        {
            c.HasCheckConstraint(
                    "CK_Countries_IsoA2_Length",
                    "LEN([Iso31661A2Code]) = 2");
            c.HasCheckConstraint(
                    "CK_Countries_IsoA3_Length",
                    "LEN([Iso31661A3Code]) = 3");
            c.HasCheckConstraint(
                    "CK_Country_IsoA2_Uppercase",
                    "UPPER([Iso31661A2Code]) = [Iso31661A2Code]"
            );
            c.HasCheckConstraint(
                    "CK_Country_IsoA3_Uppercase",
                    "UPPER([Iso31661A3Code]) = [Iso31661A3Code]"
            );
            c.HasCheckConstraint(
                    "CK_Country_IsoNum_Digits",
                    "[Iso31661NumCode] NOT LIKE '%[^0-9]%'"
            );
        });

        // ------------------------------------------------------------
        // Unique index for ISO 3166-1 Alpha-2 code
        // Ensures each country has a unique 2-letter ISO code
        // ------------------------------------------------------------
        builder.HasIndex(c => c.Iso31661A2Code)
                .IsUnique()
                .HasDatabaseName("IX_Countries_Iso31661A2Code");
        // ------------------------------------------------------------
        // Unique index for ISO 3166-1 Alpha-3 code
        // Ensures each country has a unique 3-letter ISO code
        // ------------------------------------------------------------
        builder.HasIndex(c => c.Iso31661A3Code)
                .IsUnique()
                .HasDatabaseName("IX_Countries_Iso31661A3Code");
        // ------------------------------------------------------------
        // Unique index for ISO 3166-1 Numeric code
        // Ensures each country has a unique numeric ISO code
        // ------------------------------------------------------------
        builder.HasIndex(c => c.Iso31661NumCode)
                .IsUnique()
                .HasDatabaseName("IX_Countries_Iso31661NumCode");
        // ------------------------------------------------------------
        // Non-unique index for Name
        // Improves search performance when filtering by country name
        // ------------------------------------------------------------ 
        builder.HasIndex(c => c.Name)
                .HasDatabaseName("IX_Countries_Name");
        // ------------------------------------------------------------
        // Non-unique index for Domain // Useful for lookups by internet domain (e.g., '.es', '.fr')
        // ------------------------------------------------------------
        builder.HasIndex(c => c.Domain)
                .HasDatabaseName("IX_Countries_Domain");
        // ------------------------------------------------------------
        // Non-unique index for CurrencyCode // Improves filtering by currency (e.g., EUR, USD)
        // ------------------------------------------------------------
        builder.HasIndex(c => c.CurrencyCode)
                .HasDatabaseName("IX_Countries_CurrencyCode");
    }
}
