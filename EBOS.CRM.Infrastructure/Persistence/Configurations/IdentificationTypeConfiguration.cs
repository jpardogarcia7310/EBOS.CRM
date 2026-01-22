using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class IdentificationTypeConfiguration : IEntityTypeConfiguration<IdentificationType>
{
    public void Configure(EntityTypeBuilder<IdentificationType> builder)
    {
        builder.ToTable("IdentificationTypes", "CRM");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(t => t.Erased)
            .IsRequired();

        builder.HasIndex(it => it.Code)
            .IsUnique()
            .HasDatabaseName("IX_IdentificationType_Code_Unique");
    }
}
