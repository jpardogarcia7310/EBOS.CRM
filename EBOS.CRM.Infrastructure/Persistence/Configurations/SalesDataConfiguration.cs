using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations;

public class SalesDataConfiguration : IEntityTypeConfiguration<SalesData>
{
    private const string sBigint = "bigint";

    public void Configure(EntityTypeBuilder<SalesData> builder)
    {
        builder.ToTable("SalesData, CRM");
        builder.HasKey(sd => sd.Id);
        builder.Property(sd => sd.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("bigint");

        builder.Property(sd => sd.CreditDays)
            .HasColumnType("int");
        builder.Property(sd => sd.ReviewDay)
            .HasColumnType("int");
        builder.Property(sd => sd.PaymentDay)
            .HasColumnType("int");
        builder.Property(sd => sd.AccountNumber)
            .IsRequired()
            .HasMaxLength(23);
        builder.Property(sd => sd.DiscountPercentage)
            .IsRequired()
            .HasDefaultValue(0.0)
            .HasColumnType("money");
        builder.Property(sd => sd.AccountingAccount)
            .HasMaxLength(20);
        builder.Property(sd => sd.CustomerId)
            .IsRequired()
            .HasColumnType(sBigint);
        builder.Property(sd => sd.SellerId)
            .IsRequired()
            .HasColumnType(sBigint);
        builder.Property(sd => sd.PaymentMethodId)
            .IsRequired()
            .HasColumnType(sBigint);
        builder.Property(sd => sd.PriceListId)
            .IsRequired()
            .HasColumnType(sBigint);
        builder.Property(sd => sd.TemplateDocumentId)
            .IsRequired()
            .HasColumnType(sBigint);

        builder.Property(sd => sd.Erased)
            .HasDefaultValue(false)    // asegura false en nuevas filas
            .IsRequired();

        // Propiedades y constraints
        builder.HasOne(sd => sd.Customer)
               .WithOne(c => c.SalesConfiguration)
               .HasForeignKey<SalesData>(sd => sd.CustomerId);

        builder.ToTable("SalesData", "CRM", tb =>
        {
            tb.HasCheckConstraint("CK_Country_CreditDays_PositiveMultipleOf30", 
                "[CreditDays] IS NULL OR ([CreditDays] > 0 AND [CreditDays] % 30 = 0)");
        } );
    }
}