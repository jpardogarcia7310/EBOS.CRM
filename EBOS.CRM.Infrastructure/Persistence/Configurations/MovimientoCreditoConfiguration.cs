using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class MovimientoCreditoConfiguration : IEntityTypeConfiguration<MovimientoCredito>
{
    public void Configure(EntityTypeBuilder<MovimientoCredito> builder)
    {
        builder.ToTable("MovimientosCredito, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(m => m.Fecha)
            .IsRequired();
        builder.Property(m => m.Importe)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(m => m.Tipo)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(m => m.ReferenciaExterna)
            .HasMaxLength(100);
    }
}