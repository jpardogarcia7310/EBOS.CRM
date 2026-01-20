using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public sealed class CreditAccount : ErasableEntity
{
    public decimal MaxAmount { get; set; }      // Límite concedido
    public decimal UsedAmount { get; set; }   // Lo ya gastado
    public decimal AvailableAmount => MaxAmount - UsedAmount;

    // Foreign Keys
    public long CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
}
