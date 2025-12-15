using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("Statuses", "EBOS");
        builder.HasKey(tr => tr.Id);
        builder.Property(tr => tr.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(tr => tr.Description)
            .IsRequired()
            .HasMaxLength(100);
    }
}