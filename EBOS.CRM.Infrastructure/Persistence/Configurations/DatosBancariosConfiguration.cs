using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class DatosBancariosConfiguration : IEntityTypeConfiguration<DatosBancarios>
{
    public void Configure(EntityTypeBuilder<DatosBancarios> builder)
    {
        builder.ToTable("DatosBancarios, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(db => db.Iban)
            .IsRequired()
            .HasMaxLength(34);
        builder.Property(db => db.Bic)
            .HasMaxLength(11);
        builder.Property(db => db.Banco)
            .HasMaxLength(150);
    }
}
