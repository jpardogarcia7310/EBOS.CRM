using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CustomerMergeHistoryConfiguration : IEntityTypeConfiguration<CustomerMergeHistory>
{
    public void Configure(EntityTypeBuilder<CustomerMergeHistory> builder)
    {
        builder.ToTable("CustomerMergeHistories", "CRM");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.WinnerCustomerId).IsRequired();
        builder.Property(x => x.MergedCustomerId).IsRequired();
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(500);
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.Erased).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.WinnerCustomerId, x.CreatedAt })
            .HasDatabaseName("IX_CustomerMergeHistory_Tenant_Winner_CreatedAt");

        builder.HasIndex(x => new { x.TenantId, x.MergedCustomerId })
            .HasDatabaseName("IX_CustomerMergeHistory_Tenant_Merged");

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.WinnerCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.MergedCustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
