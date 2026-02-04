using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

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
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(m => m.CreatedBy)
            .IsRequired();
        builder.Property(m => m.UpdatedAt);
        builder.Property(m => m.UpdatedBy);
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

        builder.HasIndex(ct => new { ct.Date, ct.CreditAccountId })
            .HasDatabaseName("IX_CreditTransaction_Date_Account");
        builder.HasIndex(ct => new { ct.CreditAccountId, ct.Date })
            .HasDatabaseName("IX_CreditTransaction_Account_Date");

        // ------------------------------------------------------------
        // One-to-Many: CreditAccount (principal) → CreditTransactions (dependent)
        // FK: CreditTransactions.CreditAccountId
        // ------------------------------------------------------------
        builder.HasOne(m => m.CreditAccount)
            .WithMany(c => c.CreditTransactions)
            .HasForeignKey(m => m.CreditAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for FK: CreditTransactions.CreditAccountId
        builder.HasIndex(m => m.CreditAccountId)
            .HasDatabaseName("IX_CreditTransactions_CreditAccountId");
    }
}
