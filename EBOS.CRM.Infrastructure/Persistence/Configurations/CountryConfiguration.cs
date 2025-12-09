using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        // Declaración de la tabla con sus restricciones adicionales (ejemplo: comprobar formato si se desea)
        // Nota: EF no valida expresiones complejas en DB; si quieres constraints a nivel BD,
        // puedes añadir CHECK constraints con HasCheckConstraint:
        builder.ToTable("Countries", tb =>
        {
            tb.HasCheckConstraint("CK_Countries_IsoA2_Length", "LEN([Iso31661A2Code]) = 2");
            tb.HasCheckConstraint("CK_Countries_IsoA3_Length", "LEN([Iso31661A3Code]) = 3");
        });

        // Key
        builder.HasKey(x => x.Id);

        // Propiedades y constraints
        builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

        builder.Property(x => x.Iso31661A2Code)
                .IsRequired()
                .HasMaxLength(2)
                .HasColumnType("nvarchar(2)");

        builder.Property(x => x.Iso31661A3Code)
                .IsRequired()
                .HasMaxLength(3)
                .HasColumnType("nvarchar(3)");

        builder.Property(x => x.Iso31661NumCode)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("nvarchar(10)");

        builder.Property(x => x.Domain)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");

        builder.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

        builder.Property(x => x.CurrencyCode)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnType("nvarchar(10)");

        builder.Property(x => x.InternationalPhoneCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("nvarchar(20)");

        // Índices recomendados
        builder.HasIndex(x => x.Iso31661A2Code)
                .IsUnique()
                .HasDatabaseName("IX_Countries_Iso31661A2Code");

        builder.HasIndex(x => x.Iso31661A3Code)
                .IsUnique()
                .HasDatabaseName("IX_Countries_Iso31661A3Code");

        builder.HasIndex(x => x.Iso31661NumCode)
                .IsUnique()
                .HasDatabaseName("IX_Countries_Iso31661NumCode");

        // Índices no únicos para búsquedas frecuentes
        builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_Countries_Name");

        builder.HasIndex(x => x.Domain)
                .HasDatabaseName("IX_Countries_Domain");

        builder.HasIndex(x => x.CurrencyCode)
                .HasDatabaseName("IX_Countries_CurrencyCode");

    }
}
