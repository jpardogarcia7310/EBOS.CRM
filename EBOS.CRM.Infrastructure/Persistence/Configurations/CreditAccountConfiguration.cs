using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CreditAccountConfiguration : IEntityTypeConfiguration<CreditAccount>
{
    public void Configure(EntityTypeBuilder<CreditAccount> builder)
    {
        builder.ToTable("CreditAccounts", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
               .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(c => c.MaxAmount)
               .IsRequired()
               .HasPrecision(18, 2);
        builder.Property(c => c.UsedAmount)
               .IsRequired()
               .HasPrecision(18, 2);
        builder.Property(c => c.Erased)
               .IsRequired();

        // ------------------------------------------------------------
        // One-to-One: Cliente (principal) → Credito (dependent)
        // FK: Credito.ClienteId
        //
        // Cliente has a navigation property "Credito"
        // Credito has a navigation property "Cliente"
        // ------------------------------------------------------------
        builder.HasOne(c => c.Customer)
               .WithOne(cl => cl.CreditAccount)
               .HasForeignKey<CreditAccount>(c => c.ClienteId)
               .OnDelete(DeleteBehavior.Cascade);
        // Index for FK: Credito.ClienteId
        builder.HasIndex(c => c.ClienteId)
               .HasDatabaseName("IX_CreditAccount_CustomerId");
        // ------------------------------------------------------------
        // One-to-Many: Credito (principal) → MovimientoCredito (dependent)
        // FK: MovimientoCredito.CreditoId
        // ------------------------------------------------------------
        builder.HasMany(c => c.CreditTransactions)
               .WithOne(m => m.CreditAccount)
               .HasForeignKey(m => m.CreditoId)
               .OnDelete(DeleteBehavior.Cascade);
        // No index here because FK belongs to MovimientoCredito.
        // The index is created in MovimientoCreditoConfiguration.
    }
}
