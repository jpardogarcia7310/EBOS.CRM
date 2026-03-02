using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class AccountContactConfiguration : IEntityTypeConfiguration<AccountContact>
{
    public void Configure(EntityTypeBuilder<AccountContact> builder)
    {
        builder.ToTable("AccountContacts", "CRM");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.CorporateCustomerId).IsRequired();
        builder.Property(x => x.IndividualCustomerId).IsRequired();
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.StartAt).IsRequired();
        builder.Property(x => x.EndAt);
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.UpdatedBy);
        builder.Property(x => x.Erased).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.CorporateCustomerId })
            .HasDatabaseName("IX_AccountContact_TenantId_CorporateCustomerId");
        builder.HasIndex(x => new { x.TenantId, x.IndividualCustomerId })
            .HasDatabaseName("IX_AccountContact_TenantId_IndividualCustomerId");
        builder.HasIndex(x => new { x.TenantId, x.CorporateCustomerId, x.IndividualCustomerId })
            .IsUnique()
            .HasFilter("[Erased] = 0")
            .HasDatabaseName("UX_AccountContact_Tenant_Corporate_Individual_Active");
        builder.HasIndex(x => new { x.TenantId, x.CorporateCustomerId })
            .IsUnique()
            .HasFilter("[IsPrimary] = 1 AND [Erased] = 0")
            .HasDatabaseName("UX_AccountContact_Tenant_Corporate_Primary_Active");

        builder.HasOne(x => x.CorporateCustomer)
            .WithMany(c => c.AccountContacts)
            .HasForeignKey(x => x.CorporateCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.IndividualCustomer)
            .WithMany(c => c.AccountContacts)
            .HasForeignKey(x => x.IndividualCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Roles)
            .WithOne(r => r.AccountContact)
            .HasForeignKey(r => r.AccountContactId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
