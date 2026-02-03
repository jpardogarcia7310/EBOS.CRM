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
        builder.Property(bo => bo.Erased)
            .IsRequired();

        builder.HasOne(bo => bo.CorporateCustomer)
            .WithMany(cc => cc.BranchOffices)
            .HasForeignKey(bo => bo.CorporateCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(bo => bo.CorporateCustomerId)
            .HasDatabaseName("IX_BranchOffice_CorporateCustomerId");
    }
}
