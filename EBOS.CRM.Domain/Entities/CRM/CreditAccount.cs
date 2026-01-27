using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.CRM;

public sealed class CreditAccount : ErasableEntity
{
    public decimal MaxAmount { get; set; }  // Limit granted
    public decimal UsedAmount { get; set; } // What has already been spent
    public decimal AvailableAmount => MaxAmount - UsedAmount;

    // Foreign Keys
    public long CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
}
