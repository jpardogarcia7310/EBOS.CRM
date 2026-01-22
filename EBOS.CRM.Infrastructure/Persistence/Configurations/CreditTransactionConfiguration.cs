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

        builder.ToTable("CreditTransactions", "CRM", ct =>
        {
            ct.HasCheckConstraint(
                "CK_CreditTransaction_Amount_NotZero",
                "[Amount] <> 0"
            );
            ct.HasCheckConstraint(
                "CK_CreditTransaction_Type_Valid",
                "[Type] IN ('Consumo', 'Ajuste', 'Devolucion')"
            );

        });
        
        builder.HasIndex(ct => new { ct.CreditAccountId, ct.Date })
            .HasDatabaseName("IX_CreditTransaction_Account_Date");
        builder.HasIndex(ct => ct.Date) 
            .HasDatabaseName("IX_CreditTransaction_Date"); 
        builder.HasIndex(ct => ct.CreditAccountId) 
            .HasDatabaseName("IX_CreditTransaction_Account"); 

        // ------------------------------------------------------------
        // One-to-Many: CreditAccount (principal) → CreditTransactions (dependent)
        // FK: CreditTransactions.CreditAccountId
        // ------------------------------------------------------------
        builder.HasOne(m => m.CreditAccount)
            .WithMany(c => c.CreditTransactions)
            .HasForeignKey(m => m.CreditAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for FK: CreditTransactions.CreditAccountId
        builder.HasIndex(m => m.CreditAccountId)
            .HasDatabaseName("IX_CreditTransactions_CreditAccountId");
    }
}