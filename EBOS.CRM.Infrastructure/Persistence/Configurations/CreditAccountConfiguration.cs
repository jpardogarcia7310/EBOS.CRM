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
        builder.HasKey(ca => ca.Id);
        builder.Property(ca => ca.Id)
               .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(ca => ca.MaxAmount)
               .IsRequired()
               .HasPrecision(18, 2);
        builder.Property(ca => ca.UsedAmount)
               .IsRequired()
               .HasPrecision(18, 2);
        builder.Ignore(ca => ca.AvailableAmount); 
        builder.Property(ca => ca.Erased)
               .IsRequired();

        builder.ToTable("CreditAccounts", "CRM", ca =>
        {
               ca.HasCheckConstraint(
                      "CK_CreditAccount_MaxAmount_Positive",
                      "[MaxAmount] > 0"
               );
               ca.HasCheckConstraint(
                      "CK_CreditAccount_UsedAmount_NonNegative",
                      "[UsedAmount] >= 0"
               );
               ca.HasCheckConstraint(
                      "CK_CreditAccount_UsedAmount_WithinLimit",
                      "[UsedAmount] <= [MaxAmount]"
               );
        });
        
        builder.HasIndex(ca => ca.CustomerId) 
               .IsUnique() 
               .HasDatabaseName("IX_CreditAccount_Customer_Unique");    
        
        // ------------------------------------------------------------
        // One-to-One: Customer (principal) → CreditAccount (dependent)
        // FK: CreditAccount.CustomerId
        // ------------------------------------------------------------
        builder.HasOne(ca => ca.Customer)
               .WithOne(c => c.CreditAccount)
               .HasForeignKey<CreditAccount>(ca => ca.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(ca => ca.CustomerId)
               .HasDatabaseName("IX_CreditAccount_CustomerId");
        // ------------------------------------------------------------
        // One-to-Many: CreditAccount (principal) → CreditTransactions (dependent)
        // FK: CreditTransactions.CreditAccountId
        // ------------------------------------------------------------
        builder.HasMany(ca => ca.CreditTransactions)
               .WithOne(ct => ct.CreditAccount)
               .HasForeignKey(ct => ct.CreditAccountId)
               .OnDelete(DeleteBehavior.Restrict);
        
        // ------------------------------------------------------------
        // One-to-One: CreditAccount (principal) → Customer (dependent)
        // FK: CreditAccount.CustomerId
        // ------------------------------------------------------------
        builder.HasOne(ca => ca.Customer)
               .WithOne(c => c.CreditAccount)
               .HasForeignKey<CreditAccount>(ca => ca.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
