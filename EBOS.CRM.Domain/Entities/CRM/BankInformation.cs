using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces;

namespace EBOS.CRM.Domain.Entities.CRM;

public class BankInformation : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    [Required]
    [MaxLength(34)]
    public string Iban { get; set; } = null!;
    [MaxLength(11)]
    public string? Bic { get; set; }
    [MaxLength(200)]
    public string? BankName { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    // Foreign Keys
    public long CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;
}


