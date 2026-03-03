using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CustomerPrivacyRequestConfiguration : IEntityTypeConfiguration<CustomerPrivacyRequest>
{
    public void Configure(EntityTypeBuilder<CustomerPrivacyRequest> builder)
    {
        builder.ToTable("CustomerPrivacyRequests", "CRM");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.RequestType).IsRequired().HasMaxLength(40);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(40);
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.RequestedBy).IsRequired();
        builder.Property(x => x.RequestedAt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.ProcessedBy);
        builder.Property(x => x.ProcessedAt);
        builder.Property(x => x.FailureCode).HasMaxLength(100);
        builder.Property(x => x.FailureReason).HasMaxLength(2000);
        builder.Property(x => x.CorrelationId).HasMaxLength(100);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.Property(x => x.Erased).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.CustomerId, x.RequestedAt })
            .HasDatabaseName("IX_CustomerPrivacyRequest_Tenant_Customer_RequestedAt");

        builder.HasIndex(x => new { x.TenantId, x.Status, x.RequestedAt })
            .HasDatabaseName("IX_CustomerPrivacyRequest_Tenant_Status_RequestedAt");

        builder.HasIndex(x => new { x.TenantId, x.CustomerId, x.RequestType })
            .IsUnique()
            .HasFilter($"[Erased] = 0 AND [Status] IN ('{CustomerPrivacyRequest.StatusPending}','{CustomerPrivacyRequest.StatusInProgress}')")
            .HasDatabaseName("UX_CustomerPrivacyRequest_ActiveByType");

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
