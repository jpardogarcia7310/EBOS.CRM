using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CreditTransactionConfiguration : IEntityTypeConfiguration<CreditTransaction>
{
    public void Configure(EntityTypeBuilder<CreditTransaction> builder)
    {
        builder.ToTable("CreditTransactions", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();
        
        // Basic properties
        builder.Property(m => m.Date)
            .IsRequired();
        builder.Property(m => m.Amount)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(m => m.Type)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(m => m.ExternalReference)
            .HasMaxLength(200);
        builder.Property(m => m.Comments)
            .HasMaxLength(500);
        builder.Property(c => c.Erased)
            .IsRequired();

        // ------------------------------------------------------------
        // One-to-Many: Credito (principal) → MovimientoCredito (dependent)
        // FK: MovimientoCredito.CreditoId
        // ------------------------------------------------------------
        builder.HasOne(m => m.CreditAccount)
            .WithMany(c => c.CreditTransactions)
            .HasForeignKey(m => m.CreditoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for FK: MovimientoCredito.CreditoId
        builder.HasIndex(m => m.CreditoId)
            .HasDatabaseName("IX_CreditTransactions_CreditAccountId");
    }
}