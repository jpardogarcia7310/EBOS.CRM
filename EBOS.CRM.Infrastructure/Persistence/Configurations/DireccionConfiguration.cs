using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class DireccionConfiguration : IEntityTypeConfiguration<Direccion>
{
    public void Configure(EntityTypeBuilder<Direccion> builder)
    {
        builder.ToTable("Direcciones, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(d => d.Tipo)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(d => d.Calle)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(d => d.NumeroExterno)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(d => d.NumeroInterno)
            .HasMaxLength(20);
        builder.Property(d => d.EntreCalle1)
            .HasMaxLength(200);
        builder.Property(d => d.EntreCalle2)
            .HasMaxLength(200);
        builder.Property(d => d.Barrio)
            .HasMaxLength(150);
        builder.Property(d => d.Localidad)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(d => d.Provincia)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(d => d.Pais)
            .IsRequired()
            .HasMaxLength(150);
        builder.Property(d => d.CodigoPostal)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(d => d.UrlGoogleMaps)
            .HasMaxLength(500);
        builder.Property(d => d.Latitud)
            .HasColumnType("decimal(10,8)");
        builder.Property(d => d.Longitud)
            .HasColumnType("decimal(11,8)");

        builder.HasOne(d => d.Cliente)
            .WithMany(c => c.Direcciones)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}