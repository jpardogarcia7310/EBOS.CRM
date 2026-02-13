using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "CRM");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(12);
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(c => c.CreatedBy)
            .IsRequired();
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.UpdatedBy);
        builder.Property(c => c.Erased)
            .IsRequired();

        builder.ToTable("Customers", "CRM", c =>
        {
            c.HasCheckConstraint(
                "CK_Customer_Email_Valid",
                "[Email] LIKE '%@%.%'");
            c.HasCheckConstraint(
                "CK_Customer_Phone_Digits",
                "[Phone] NOT LIKE '%[^0-9]%'");
        });

        builder.HasIndex(c => new { c.StatusId, c.CreatedAt })
            .HasDatabaseName("IX_Customer_Status_CreatedAt");
        builder.HasIndex(c => c.TenantId)
            .HasDatabaseName("IX_Customer_TenantId");
        builder.HasIndex(c => new { c.TenantId, c.Code })
            .IsUnique()
            .HasDatabaseName("UX_Customer_TenantId_Code");
        builder.HasIndex(c => new { c.TenantId, c.Email })
            .HasDatabaseName("IX_Customer_TenantId_Email");
        builder.HasIndex(c => new { c.TenantId, c.Phone })
            .HasDatabaseName("IX_Customer_TenantId_Phone");

        builder.HasOne(c => c.Status)
            .WithMany(s => s.Customers)
            .HasForeignKey(c => c.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CreditAccount)
            .WithOne(cr => cr.Customer)
            .HasForeignKey<CreditAccount>(cr => cr.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.TaxInformation)
            .WithOne(t => t.Customer)
            .HasForeignKey<TaxInformation>(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.BankInformation)
            .WithOne(b => b.Customer)
            .HasForeignKey<BankInformation>(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasDiscriminator<string>("CustomerType")
            .HasValue<CorporateCustomer>("Corporate")
            .HasValue<IndividualCustomer>("Individual");
    }
}

