using EBOS.Core.Primitives;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("SalesData, CRM")]
public class SalesData : ErasableEntity
{
    [Required]
    [DefaultValue(false)]
    public bool HasCredit { get; set; }

    public int? CreditDays { get; set; } // multiples of 30
    public int? ReviewDay { get; set; }
    public int? PaymentDay { get; set; }
    [Required]
    [MaxLength(23)]
    public string AccountNumber { get; set; } = string.Empty;
    [Required]
    [DefaultValue(0.0)]
    public decimal DiscountPercentage { get; set; }
    [MaxLength(20)]
    public string? AccountingAccount { get; set; } = string.Empty;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public long SellerId { get; set; }
    public long PaymentMethodId { get; set; }
    public long PriceListId { get; set; }
    public long TemplateDocumentId { get; set; }
}