using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.ToTable("Quotes", "CRM");

        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).ValueGeneratedOnAdd();

        builder.Property(q => q.Status)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(q => q.ReferenceNumber)
            .HasMaxLength(50);
        builder.Property(q => q.SubtotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(q => q.DiscountAmount)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(q => q.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);
        builder.Property(q => q.Notes)
            .HasMaxLength(2000);
        builder.Property(q => q.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(q => q.CreatedBy)
            .IsRequired();
        builder.Property(q => q.UpdatedAt);
        builder.Property(q => q.UpdatedBy);
        builder.Property(q => q.Erased)
            .IsRequired();

        builder.ToTable("Quotes", "CRM", q =>
        {
            q.HasCheckConstraint(
                "CK_Quote_Subtotal_NonNegative",
                "[SubtotalAmount] >= 0");
            q.HasCheckConstraint(
                "CK_Quote_Discount_NonNegative",
                "[DiscountAmount] >= 0");
            q.HasCheckConstraint(
                "CK_Quote_Discount_Lte_Subtotal",
                "[DiscountAmount] <= [SubtotalAmount]");
            q.HasCheckConstraint(
                "CK_Quote_Total_NonNegative",
                "[TotalAmount] >= 0");
        });

        builder.HasOne(q => q.Opportunity)
            .WithMany(o => o.Quotes)
            .HasForeignKey(q => q.OpportunityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(q => q.TenantId)
            .HasDatabaseName("IX_Quote_TenantId");
        builder.HasIndex(q => new { q.OpportunityId, q.Status })
            .HasDatabaseName("IX_Quote_Opportunity_Status");
        builder.HasIndex(q => new { q.TenantId, q.ReferenceNumber })
            .HasDatabaseName("UX_Quote_TenantId_ReferenceNumber")
            .IsUnique()
            .HasFilter("[ReferenceNumber] IS NOT NULL");
    }
}
