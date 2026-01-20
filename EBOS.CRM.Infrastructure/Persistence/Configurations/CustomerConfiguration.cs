using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
               .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(c => c.Code)
               .IsRequired()
               .HasMaxLength(50);
        builder.Property(c => c.Email)
               .IsRequired()
               .HasMaxLength(100);
        builder.Property(c => c.Phone)
               .IsRequired()
               .HasMaxLength(12);
        builder.Property(c => c.CreatedAt)
               .IsRequired();
        builder.Property(c => c.Erased)
               .IsRequired();

        // ------------------------------------------------------------
        // One-to-Many: Cliente (principal) → Direccion (dependent)
        // FK: Direccion.ClienteId
        // ------------------------------------------------------------
        builder.HasMany(c => c.Addresses)
               .WithOne(d => d.Customer)
               .HasForeignKey(d => d.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        // Index for FK: Direccion.ClienteId
        builder.HasIndex(c => c.StatusId);

        // ------------------------------------------------------------
        // One-to-Many: Estado (principal) → Cliente (dependent)
        // FK: Cliente.EstadoId
        // ------------------------------------------------------------
        builder.HasOne(c => c.Status)
               .WithMany(e => e.Customers)
               .HasForeignKey(c => c.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

        // Index for FK: Cliente.EstadoId
        builder.HasIndex(c => c.StatusId);

        // ------------------------------------------------------------
        // One-to-One: DatosFiscales (principal) → Cliente (dependent)
        // FK: Cliente.DatosFiscalesId
        // ------------------------------------------------------------
        builder.HasOne(c => c.TaxInformation)
               .WithOne(df => df.Customer)
               .HasForeignKey<Customer>(c => c.TaxInformationId)
               .OnDelete(DeleteBehavior.SetNull);

        // Index for FK: Cliente.DatosFiscalesId
        builder.HasIndex(c => c.TaxInformationId);

        // ------------------------------------------------------------
        // One-to-One: DatosBancarios (principal) → Cliente (dependent)
        // FK: Cliente.DatosBancariosId
        // ------------------------------------------------------------
        builder.HasOne(c => c.BankInformation)
               .WithOne(db => db.Customer)
               .HasForeignKey<Customer>(c => c.BankInformationId)
               .OnDelete(DeleteBehavior.SetNull);

        // Index for FK: Cliente.DatosBancariosId
        builder.HasIndex(c => c.BankInformationId);

        // ------------------------------------------------------------
        // One-to-One: Cliente (principal) → Credito (dependent)
        // FK: Credito.ClienteId
        // ------------------------------------------------------------
        builder.HasOne(c => c.CreditAccount)
               .WithOne(cr => cr.Customer)
               .HasForeignKey<CreditAccount>(cr => cr.ClienteId)
               .OnDelete(DeleteBehavior.Cascade);

        // TPH inheritance discriminator
        builder.HasDiscriminator<string>("TipoCliente")
               .HasValue<CorporateCustomer>("Empresa")
               .HasValue<IndividualCustomer>("PersonaFisica");
    }
}
