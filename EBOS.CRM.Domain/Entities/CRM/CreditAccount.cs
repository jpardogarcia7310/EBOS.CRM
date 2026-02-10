using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public sealed class CreditAccount : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public decimal MaxAmount { get; set; }  // Limit granted
    public decimal UsedAmount { get; set; } // What has already been spent
    public decimal AvailableAmount => MaxAmount - UsedAmount;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    // Foreign Keys
    public long CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
}


