using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "CRM");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("nvarchar(255)");
        builder.Property(c => c.Balance)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("mnoney(18,2)");
        builder.Property(c => c.IsCompany)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnType("bit");
        builder.Property(c => c.CompanyType)
            .HasDefaultValue(true)
            .HasColumnType("bit");
        builder.Property(c => c.RFC)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");
        builder.Property(c => c.CURP)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");
        builder.Property(c => c.TaxDuplicateShippingAddress)
            .HasDefaultValue(false)
            .IsRequired()
            .HasColumnType("bit");
        builder.Property(c => c.StatusId)
            .IsRequired()
            .HasColumnType("long");
        builder.Property(c => c.TaxRegimeId)
            .IsRequired();
        builder.Property(c => c.TaxAddressId)
            .IsRequired()
            .HasColumnType("long");
        builder.Property(c => c.ShippingAddressId)
            .HasColumnType("long");
        builder.Property(c => c.SalesConfigurationId)
            .IsRequired()
            .HasColumnType("long");
        builder.Property(c => c.CustomerHistoryId)
            .IsRequired()
            .HasColumnType("long");

        builder.Property(s => s.Erased)
            .HasDefaultValue(false)    // asegura false en nuevas filas
            .IsRequired();

        // Propiedades y constraints
        builder.HasOne(c => c.Status)
               .WithMany(s => s.Customers)
               .HasForeignKey(c => c.StatusId);
        builder.HasOne(c => c.TaxRegime)
               .WithMany(tr => tr.Customers)
               .HasForeignKey(c => c.TaxRegimeId);
        builder.HasOne(c => c.TaxAddress)
               .WithOne(ta => ta.Customer)
               .HasForeignKey<TaxAddress>(ta => ta.CustomerId);
        builder.HasOne(c => c.ShippingAddress)
               .WithOne(sa => sa.Customer)
               .HasForeignKey<ShippingAddress>(sa => sa.CustomerId);
        builder.HasOne(c => c.SalesConfiguration)
               .WithOne(s => s.Customer)
               .HasForeignKey<SalesData>(s => s.CustomerId);
        builder.HasOne(c => c.CustomerHistory)
               .WithOne(ch => ch.Customer)
               .HasForeignKey<CustomerHistory>(ch => ch.CustomerId);
    }
}