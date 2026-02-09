using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CustomerAddresses", "CRM");

        builder.HasKey(ca => ca.Id);
        builder.Property(ca => ca.Id).ValueGeneratedOnAdd();

        builder.Property(ca => ca.IsPrimary)
            .IsRequired();
        builder.Property(ca => ca.ValidFrom)
            .IsRequired();
        builder.Property(ca => ca.IsCurrent)
            .IsRequired();
        builder.Property(ca => ca.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(ca => ca.CreatedBy)
            .IsRequired();
        builder.Property(ca => ca.UpdatedAt);
        builder.Property(ca => ca.UpdatedBy);
        builder.Property(ca => ca.Erased)
            .IsRequired();

        builder.HasOne(ca => ca.Customer)
            .WithMany(c => c.CustomerAddresses)
            .HasForeignKey(ca => ca.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ca => ca.Address)
            .WithMany(a => a.CustomerAddresses)
            .HasForeignKey(ca => ca.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ca => new { ca.CustomerId, ca.IsCurrent, ca.IsPrimary })
            .HasDatabaseName("IX_CustomerAddress_Current_Primary");
        builder.HasIndex(ca => ca.TenantId)
            .HasDatabaseName("IX_CustomerAddress_TenantId");

        builder.ToTable("CustomerAddresses", "CRM", ca =>
        {
            ca.HasCheckConstraint(
                "CK_CustomerAddress_ValidFrom_NotNull",
                "[ValidFrom] IS NOT NULL");
        });
    }
}
