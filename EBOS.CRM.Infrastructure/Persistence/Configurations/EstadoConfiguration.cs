using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> builder)
    {
        builder.ToTable("Estados", "EBOS");
        builder.HasKey(tr => tr.Id);
        builder.Property(tr => tr.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(tr => tr.Description)
            .IsRequired()
            .HasMaxLength(100);
    }
}