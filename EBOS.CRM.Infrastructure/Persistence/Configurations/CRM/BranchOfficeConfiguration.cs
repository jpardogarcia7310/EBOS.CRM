using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class BranchOfficeConfiguration : IEntityTypeConfiguration<BranchOffice>
{
    public void Configure(EntityTypeBuilder<BranchOffice> builder)
    {
        builder.ToTable("BranchOffices", "CRM");

        builder.HasKey(bo => bo.Id);
        builder.Property(bo => bo.Id).ValueGeneratedOnAdd();

        builder.Property(bo => bo.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(bo => bo.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(bo => bo.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(bo => bo.CreatedBy)
            .IsRequired();
        builder.Property(bo => bo.UpdatedAt);
        builder.Property(bo => bo.UpdatedBy);
        builder.Property(bo => bo.Erased)
            .IsRequired();

        builder.HasOne(bo => bo.CorporateCustomer)
            .WithMany(cc => cc.BranchOffices)
            .HasForeignKey(bo => bo.CorporateCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(bo => bo.CorporateCustomerId)
            .HasDatabaseName("IX_BranchOffice_CorporateCustomerId");
        builder.HasIndex(bo => bo.TenantId)
            .HasDatabaseName("IX_BranchOffice_TenantId");
    }
}
