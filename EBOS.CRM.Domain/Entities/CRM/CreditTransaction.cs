using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public sealed class CreditTransaction : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; } // Positive: consumption, Negative: refund/adjustment
    public string Type { get; set; } = null!; // "Consumption", "Adjustment", etc.
    [Required]
    [MaxLength(200)]
    public string? ExternalReference { get; set; } // Order, invoice, etc.
    [Required]
    [MaxLength(500)]
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    // Foreign Keys
    public long CreditAccountId { get; set; }
    [ForeignKey(nameof(CreditAccountId))]
    public CreditAccount CreditAccount { get; set; } = null!;
}


