using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes, CRM");

        builder.HasKey(d => d.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(c => c.Codigo)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(c => c.Telefono)
            .HasMaxLength(50);
        builder.Property(c => c.FechaAlta)
            .IsRequired();
        // Herencia TPH (Table Per Hierarchy)
        builder.HasDiscriminator<string>("TipoCliente")
            .HasValue<PersonaFisica>("PersonaFisica")
            .HasValue<Empresa>("Empresa");
        // Relaciones 1:1 con datos fiscales y bancarios
        builder.HasOne(c => c.DatosFiscales)
            .WithOne(df => df.Cliente)
            .HasForeignKey<Cliente>(c => c.DatosFiscalesId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.DatosBancarios)
            .WithOne(db => db.Cliente)
            .HasForeignKey<Cliente>(c => c.DatosBancariosId)
            .OnDelete(DeleteBehavior.Restrict);
        // Relación 1:1 con crédito
        builder.HasOne(c => c.Credito)
            .WithOne(cr => cr.Cliente)
            .HasForeignKey<Credito>(cr => cr.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
        // Relacion 1: N con direccion
        builder.HasMany(c => c.Direcciones)
            .WithOne(d => d.Cliente)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
