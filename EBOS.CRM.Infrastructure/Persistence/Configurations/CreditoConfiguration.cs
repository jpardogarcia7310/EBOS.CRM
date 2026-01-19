using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class CreditoConfiguration : IEntityTypeConfiguration<Credito>
{
    public void Configure(EntityTypeBuilder<Credito> builder)
    {
        builder.ToTable("Creditos, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(c => c.ImporteMaximo)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Property(c => c.ImporteConsumido)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasMany(c => c.Movimientos)
            .WithOne(m => m.Credito)
            .HasForeignKey(m => m.CreditoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
