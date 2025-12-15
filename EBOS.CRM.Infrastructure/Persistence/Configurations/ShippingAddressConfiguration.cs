using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
{
    private const string sNVarchar50 = "nvarchar(50)";

    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.ToTable("ShippingAddresses", "CRM");
        builder.HasKey(sa => sa.Id);
        builder.Property(sa => sa.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(sa => sa.Street)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("nvarchar(255)");
        builder.Property(sa => sa.ExternalNumber)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("nvarchar(10)");
        builder.Property(sa => sa.InternalNumber)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("nvarchar(10)");
        builder.Property(sa => sa.PostalCode)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("nvarchar(10)");
        builder.Property(sa => sa.State)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType(sNVarchar50);
        builder.Property(sa => sa.Municipality)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType(sNVarchar50);
        builder.Property(sa => sa.City)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType(sNVarchar50);
        builder.Property(sa => sa.Neighborhood)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType(sNVarchar50);
        builder.Property(sa => sa.Reference)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");
        builder.Property(sa => sa.CustomerId)
            .IsRequired()
            .HasColumnType("bigint");
        builder.Property(sa => sa.CountryId)
            .IsRequired()
            .HasColumnType("bigint");

        builder.Property(sa => sa.Erased)
            .HasDefaultValue(false)    // asegura false en nuevas filas
            .IsRequired();

        // Propiedades y constraints
        builder.HasOne(sa => sa.Customer)
            .WithOne(c => c.ShippingAddress)
            .HasForeignKey<ShippingAddress>(sa => sa.CustomerId);
        builder.HasOne(sa => sa.Country)
           .WithMany(co => co.ShippingAddresses)
           .HasForeignKey(sa => sa.CountryId);
    }
}