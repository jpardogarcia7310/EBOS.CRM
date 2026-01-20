using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
               .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(d => d.AddressType)
               .HasMaxLength(50);
        builder.Property(d => d.Street)
               .IsRequired()
               .HasMaxLength(200);
        builder.Property(d => d.ExternalNumber)
               .IsRequired()
               .HasMaxLength(20);
        builder.Property(d => d.InternalNumber)
               .HasMaxLength(20);
        builder.Property(d => d.BetweenStreet1)
               .HasMaxLength(200);
        builder.Property(d => d.BetweenStreet2)
               .HasMaxLength(200);
        builder.Property(d => d.Neighborhood)
               .HasMaxLength(200);
        builder.Property(d => d.City)
               .IsRequired()
               .HasMaxLength(150);
        builder.Property(d => d.StateOrProvince)
               .IsRequired()
               .HasMaxLength(150);
        builder.Property(d => d.PostalCode)
               .IsRequired()
               .HasMaxLength(20);
        builder.Property(d => d.GoogleMapsUrl)
               .HasMaxLength(500);
        // Geolocation properties
        builder.Property(d => d.Latitude)
               .HasPrecision(10, 6);
        builder.Property(d => d.Longitude)
               .HasPrecision(10, 6);
        builder.Property(c => c.Erased)
               .IsRequired();

        // ------------------------------------------------------------
        // One-to-Many: Cliente (principal) → Direccion (dependent)
        // FK: Direccion.ClienteId
        // ------------------------------------------------------------
        builder.HasOne(d => d.Customer)
               .WithMany(c => c.Addresses)
               .HasForeignKey(d => d.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
        // Index for FK: Direccion.ClienteId
        builder.HasIndex(d => d.CustomerId)
               .HasDatabaseName("IX_Address_CustomerId");

        // ------------------------------------------------------------
        // One-to-Many: Pais (principal) → Direccion (dependent)
        // FK: Direccion.PaisId
        // ------------------------------------------------------------
        builder.HasOne(d => d.Country)
               .WithMany()
               .HasForeignKey(d => d.CountryId)
               .OnDelete(DeleteBehavior.Restrict);
        // Index for FK: Direccion.PaisId
        builder.HasIndex(d => d.CountryId)
               .HasDatabaseName("IX_Address_CountryId");
    }
}
