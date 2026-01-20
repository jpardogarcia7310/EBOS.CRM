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
        // One-to-One: Customer (principal) → CreditAccount (dependent)
        // FK: CreditAccount.CustomerId
        //
        // Customer has a navigation property "CreditAccount"
        // CreditAccount has a navigation property "Customer"
        // ------------------------------------------------------------
        builder.HasOne(c => c.Customer)
               .WithOne(cl => cl.CreditAccount)
               .HasForeignKey<CreditAccount>(c => c.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
        // Index for FK: Credito.ClienteId
        builder.HasIndex(c => c.CustomerId)
               .HasDatabaseName("IX_CreditAccount_CustomerId");
        // ------------------------------------------------------------
        // One-to-Many: CreditAccount (principal) → CreditTransactions (dependent)
        // FK: CreditTransactions.CreditAccountId
        // ------------------------------------------------------------
        builder.HasMany(c => c.CreditTransactions)
               .WithOne(m => m.CreditAccount)
               .HasForeignKey(m => m.CreditAccountId)
               .OnDelete(DeleteBehavior.Cascade);
        // No index here because FK belongs to CreditTransactions.
        // The index is created in CreditTransactionsConfiguration.
    }
}
