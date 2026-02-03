using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class IdentificationTypeConfiguration : IEntityTypeConfiguration<IdentificationType>
{
    public void Configure(EntityTypeBuilder<IdentificationType> builder)
    {
        builder.ToTable("IdentificationTypes", "EBOS");

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
    }
}

