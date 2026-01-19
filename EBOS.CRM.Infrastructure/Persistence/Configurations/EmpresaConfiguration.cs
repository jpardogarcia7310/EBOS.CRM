using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("Empresas, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(e => e.RazonSocial)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(e => e.Cif)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasMany(e => e.Delegaciones)
            .WithOne(d => d.Empresa)
            .HasForeignKey(d => d.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
