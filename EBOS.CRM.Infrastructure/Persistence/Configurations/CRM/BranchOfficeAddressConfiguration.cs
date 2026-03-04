using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class BranchOfficeAddressConfiguration : IEntityTypeConfiguration<BranchOfficeAddress>
{
    public void Configure(EntityTypeBuilder<BranchOfficeAddress> builder)
    {
        builder.ToTable("BranchOfficeAddresses", "CRM");

        builder.HasKey(ba => ba.Id);
        builder.Property(ba => ba.Id).ValueGeneratedOnAdd();

        builder.Property(ba => ba.IsPrimary)
            .IsRequired();
        builder.Property(ba => ba.ValidFrom)
            .IsRequired();
        builder.Property(ba => ba.IsCurrent)
            .IsRequired();
        builder.Property(ba => ba.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(ba => ba.CreatedBy)
            .IsRequired();
        builder.Property(ba => ba.UpdatedAt);
        builder.Property(ba => ba.UpdatedBy);
        builder.Property(ba => ba.Erased)
            .IsRequired();

        builder.HasOne(ba => ba.BranchOffice)
            .WithMany(bo => bo.BranchOfficeAddresses)
            .HasForeignKey(ba => ba.BranchOfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ba => ba.Address)
            .WithMany(a => a.BranchOfficeAddresses)
            .HasForeignKey(ba => ba.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ba => new { ba.BranchOfficeId, ba.IsCurrent, ba.IsPrimary })
            .HasDatabaseName("IX_BranchOfficeAddress_Current_Primary");
        builder.HasIndex(ba => ba.TenantId)
            .HasDatabaseName("IX_BranchOfficeAddress_TenantId");
    }
}
