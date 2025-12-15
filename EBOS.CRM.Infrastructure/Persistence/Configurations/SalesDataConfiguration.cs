using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class SalesDataConfiguration : IEntityTypeConfiguration<SalesData>
{
    public void Configure(EntityTypeBuilder<SalesData> builder)
    {
        builder.ToTable("SalesData");
        builder.HasKey(sd => sd.Id);

        builder.Property(sd => sd.Id).ValueGeneratedOnAdd();

        builder.HasQueryFilter(sd => !sd.Erased);

        // Propiedades y constraints
        builder.HasOne(sd => sd.Customer)
               .WithOne(c => c.SalesConfiguration)
               .HasForeignKey<SalesData>(sd => sd.CustomerId);

        //builder.HasOne(sd => sd.Seller)
        //       .WithMany()
        //       .HasForeignKey(sd => sd.SellerId);
        //builder.HasOne(sd => sd.PaymentMethod)
        //       .WithMany()
        //       .HasForeignKey(sd => sd.PaymentMethodId);
        //builder.HasOne(sd => sd.PriceList)
        //       .WithMany()
        //       .HasForeignKey(sd => sd.PriceListId);
        //builder.HasOne(sd => sd.TemplateDocument)
        //       .WithMany()
        //       .HasForeignKey(sd => sd.TemplateDocumentId);
    }
}