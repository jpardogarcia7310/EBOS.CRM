using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DelegacionConfiguration : IEntityTypeConfiguration<Delegacion>
{
    public void Configure(EntityTypeBuilder<Delegacion> builder)
    {
        builder.ToTable("Delegaciones, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(d => d.Nombre)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(d => d.Direccion)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(d => d.Ciudad)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(d => d.CodigoPostal)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(d => d.Pais)
            .IsRequired()
            .HasMaxLength(100);
    }
}
