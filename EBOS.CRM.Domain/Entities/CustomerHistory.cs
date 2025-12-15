using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("CustomerHistory, CRM")]
public class CustomerHistory : BaseEntity // read-only, no soft delete
{
    public decimal AnnualSales { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public string LastPaymentDocument { get; set; } = string.Empty;
    public decimal LastPaymentAmount { get; set; }
    public DateTime? LastSaleDate { get; set; }
    public string LastSaleDocument { get; set; } = string.Empty;
    public decimal LastSaleAmount { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}