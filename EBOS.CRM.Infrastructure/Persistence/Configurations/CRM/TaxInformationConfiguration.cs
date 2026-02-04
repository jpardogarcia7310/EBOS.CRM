using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class TaxInformationConfiguration : IEntityTypeConfiguration<TaxInformation>
{
    public void Configure(EntityTypeBuilder<TaxInformation> builder)
    {
        builder.ToTable("TaxInformation", "CRM");

        builder.HasKey(ti => ti.Id);
        builder.Property(ti => ti.Id).ValueGeneratedOnAdd();

        builder.Property(ti => ti.TaxName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(ti => ti.TaxIdentificationNumber)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(ti => ti.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(ti => ti.CreatedBy)
            .IsRequired();
        builder.Property(ti => ti.UpdatedAt);
        builder.Property(ti => ti.UpdatedBy);
        builder.Property(ti => ti.Erased)
            .IsRequired();

        builder.ToTable("TaxInformation", "CRM", ti =>
        {
            ti.HasCheckConstraint(
                "CK_TaxInformation_TIN_Valid",
                "[TaxIdentificationNumber] NOT LIKE '%[^A-Za-z0-9]%'");
        });

        builder.HasOne(ti => ti.Customer)
            .WithOne(c => c.TaxInformation)
            .HasForeignKey<TaxInformation>(ti => ti.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
