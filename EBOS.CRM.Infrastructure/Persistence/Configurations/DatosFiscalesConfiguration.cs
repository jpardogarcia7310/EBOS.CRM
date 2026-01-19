using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DatosFiscalesConfiguration : IEntityTypeConfiguration<DatosFiscales>
{
    public void Configure(EntityTypeBuilder<DatosFiscales> builder)
    {
        builder.ToTable("DatosFiscales, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(df => df.NombreFiscal)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(df => df.Nif)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(df => df.DireccionFiscal)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(df => df.Ciudad)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(df => df.CodigoPostal)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(df => df.Pais)
            .IsRequired()
            .HasMaxLength(100);
    }
}
