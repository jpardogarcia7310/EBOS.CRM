using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("Statuses", "CRM");

        // Primary Key (BIGINT IDENTITY)
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Basic properties
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(s => s.Description) 
            .IsUnique() 
            .HasDatabaseName("IX_Status_Description_Unique"); 
        
        // ------------------------------------------------------------
        // One-to-N: Status (principal) → Customer (dependent)
        // FK: Customer.StatusId
        // ------------------------------------------------------------
        builder.HasMany(s => s.Customers) 
            .WithOne(c => c.Status) 
            .HasForeignKey(c => c.StatusId) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}