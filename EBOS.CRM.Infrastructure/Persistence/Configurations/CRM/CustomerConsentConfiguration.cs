using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CustomerConsentConfiguration : IEntityTypeConfiguration<CustomerConsent>
{
    public void Configure(EntityTypeBuilder<CustomerConsent> builder)
    {
        builder.ToTable("CustomerConsents", "CRM");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.ConsentType)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Granted).IsRequired();
        builder.Property(x => x.GrantedAt).IsRequired();
        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.ExpiresAt);
        builder.Property(x => x.RevokedAt);
        builder.Property(x => x.Erased).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.CustomerId })
            .HasDatabaseName("IX_CustomerConsent_TenantId_CustomerId");
        builder.HasIndex(x => new { x.TenantId, x.CustomerId, x.ConsentType, x.GrantedAt })
            .HasDatabaseName("IX_CustomerConsent_Tenant_Customer_ConsentType_GrantedAt");

        builder.HasOne(x => x.Customer)
            .WithMany(c => c.Consents)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
