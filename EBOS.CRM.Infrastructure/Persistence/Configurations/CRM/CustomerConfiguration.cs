using EBOS.CRM.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
               .ValueGeneratedOnAdd();

        // Basic properties
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
               .IsRequired();
        builder.Property(c => c.Erased)
               .IsRequired();
        
        builder.ToTable("Customers", "CRM", c =>
        {
               c.HasCheckConstraint(
                      "CK_Customer_Email_Valid",
                      "[Email] LIKE '%@%.%'"
               );
               c.HasCheckConstraint(
                      "CK_Customer_Phone_Digits",
                      "[Phone] NOT LIKE '%[^0-9]%'"
               );
        });

        builder.HasIndex(c => new { c.StatusId, c.CreatedAt })
               .HasDatabaseName("IX_Customer_Status_CreatedAt");

        // ------------------------------------------------------------
        // One-to-Many: Customer (principal) → Address (dependent)
        // FK: Address.CustomerId
        // ------------------------------------------------------------
        builder.HasMany(c => c.Addresses)
               .WithOne(d => d.Customer)
               .HasForeignKey(d => d.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        // Index for FK: Addresses.CustomerId
        builder.HasIndex(c => c.StatusId);

        // ------------------------------------------------------------
        // One-to-Many: Customer (principal) → Address (dependent)
        // FK: Address.CustomerId
        // ------------------------------------------------------------
        builder.HasOne(c => c.PrimaryAddress)
               .WithMany()
               .HasForeignKey(c => c.PrimaryAddressId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.PrimaryAddressId)
               .HasDatabaseName("IX_Customers_PrimaryAddressId");

        // ------------------------------------------------------------
        // One-to-Many: Status (principal) → Customer (dependent)
        // FK: Customer.StatusId
        // ------------------------------------------------------------
        builder.HasOne(c => c.Status)
               .WithMany(e => e.Customers)
               .HasForeignKey(c => c.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

        // Index for FK: Customer.StatusId
        builder.HasIndex(c => c.StatusId);
        
        // ------------------------------------------------------------
        // One-to-One: Customer (principal) → CreditAccount (dependent)
        // FK: CreditAccount.CustomerId
        // ------------------------------------------------------------
        builder.HasOne(c => c.CreditAccount)
               .WithOne(cr => cr.Customer)
               .HasForeignKey<CreditAccount>(cr => cr.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
    
        // ------------------------------------------------------------
        // One-to-One: Customer (principal) → TaxInformation (dependent)
        // FK: TaxInformation.CustomerId
        // ------------------------------------------------------------
        builder.HasOne(c => c.TaxInformation) 
               .WithOne(t => t.Customer) 
               .HasForeignKey<TaxInformation>(t => t.CustomerId) 
               .OnDelete(DeleteBehavior.Restrict); 
        
        // ------------------------------------------------------------
        // One-to-One: Customer (principal) → BankInformation (dependent)
        // FK: BankInformation.CustomerId
        // ------------------------------------------------------------
        builder.HasOne(c => c.BankInformation) 
               .WithOne(b => b.Customer) 
               .HasForeignKey<BankInformation>(b => b.CustomerId) 
               .OnDelete(DeleteBehavior.Restrict);
        
        // TPH inheritance discriminator
        builder.HasDiscriminator<string>("CustomerType")
               .HasValue<CorporateCustomer>("Corporate")
               .HasValue<IndividualCustomer>("Individual");
    }
}
