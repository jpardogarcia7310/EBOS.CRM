using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class PersonaFisicaConfiguration : IEntityTypeConfiguration<PersonaFisica>
{
    public void Configure(EntityTypeBuilder<PersonaFisica> builder)
    {
        builder.ToTable("PersonasFisicas, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.Apellidos)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(p => p.DocumentoIdentidad)
            .HasMaxLength(50);
    }
}
