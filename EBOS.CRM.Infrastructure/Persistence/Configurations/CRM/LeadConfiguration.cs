using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("Leads", "CRM");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.Source)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(l => l.Status)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(l => l.OwnerUserId)
            .IsRequired();
        builder.Property(l => l.CompanyName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(l => l.ContactName)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(l => l.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(l => l.Phone)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(l => l.EstimatedValue)
            .HasPrecision(18, 2);
        builder.Property(l => l.Notes)
            .HasMaxLength(2000);
        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(l => l.CreatedBy)
            .IsRequired();
        builder.Property(l => l.UpdatedAt);
        builder.Property(l => l.UpdatedBy);
        builder.Property(l => l.Erased)
            .IsRequired();

        builder.ToTable("Leads", "CRM", l =>
        {
            l.HasCheckConstraint(
                "CK_Lead_Email_Valid",
                "[Email] LIKE '%@%.%'");
            l.HasCheckConstraint(
                "CK_Lead_Phone_Digits",
                "[Phone] NOT LIKE '%[^0-9]%'");
            l.HasCheckConstraint(
                "CK_Lead_EstimatedValue_NonNegative",
                "[EstimatedValue] IS NULL OR [EstimatedValue] >= 0");
        });

        builder.HasOne(l => l.ConvertedOpportunity)
            .WithOne(o => o.SourceLead)
            .HasForeignKey<Opportunity>(o => o.SourceLeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.TenantId)
            .HasDatabaseName("IX_Lead_TenantId");
        builder.HasIndex(l => new { l.Status, l.CreatedAt })
            .HasDatabaseName("IX_Lead_Status_CreatedAt");
    }
}
