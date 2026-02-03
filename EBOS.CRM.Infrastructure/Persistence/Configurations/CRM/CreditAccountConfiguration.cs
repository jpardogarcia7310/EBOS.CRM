using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

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

        // ------------------------------------------------------------
        // One-to-One: Customer (principal) → CreditAccount (dependent)
        // FK: CreditAccount.CustomerId
        // ------------------------------------------------------------
        builder.HasOne(c => c.Customer)
               .WithOne(cl => cl.CreditAccount)
               .HasForeignKey<CreditAccount>(c => c.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
        // Index for FK: CreditAccount.CustomerId
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

