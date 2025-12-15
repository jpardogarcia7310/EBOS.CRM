using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class TaxAddressConfiguration : IEntityTypeConfiguration<TaxAddress>
{
    private const string sNVarchar50 = "nvarchar(50)";

    public void Configure(EntityTypeBuilder<TaxAddress> builder)
    {
        builder.ToTable("TaxAddresses", "CRM");
        builder.HasKey(ta => ta.Id);
        builder.Property(ta => ta.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(ta => ta.Street)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("nvarchar(255)");
        builder.Property(ta => ta.ExternalNumber)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("nvarchar(10)");
        builder.Property(ta => ta.InternalNumber)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("nvarchar(10)");
        builder.Property(ta => ta.PostalCode)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("nvarchar(10)");
        builder.Property(ta => ta.State)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType(sNVarchar50);
        builder.Property(ta => ta.Municipality)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType(sNVarchar50);
        builder.Property(ta => ta.City)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType(sNVarchar50);
        builder.Property(ta => ta.Neighborhood)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType(sNVarchar50);
        builder.Property(ta => ta.Reference)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");
        builder.Property(ta => ta.CustomerId)
            .IsRequired()
            .HasColumnType("bigint");
        builder.Property(ta => ta.CountryId)
            .IsRequired()
            .HasColumnType("bigint");

        builder.Property(ta => ta.Erased)
            .HasDefaultValue(false)    // asegura false en nuevas filas
            .IsRequired();

        // Propiedades y constraints
        builder.HasOne(ta => ta.Customer)
            .WithOne(c => c.TaxAddress)
            .HasForeignKey<TaxAddress>(ta => ta.CustomerId);
        builder.HasOne(ta => ta.Country)
           .WithMany(co => co.TaxAddresses)
           .HasForeignKey(ta => ta.CountryId);
    }
}