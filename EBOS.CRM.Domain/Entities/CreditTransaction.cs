using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("CreditTransactions, CRM")]
public class CreditTransaction : ErasableEntity
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; } // Positivo: consumo, Negativo: devolución/ajuste
    public string Type { get; set; } = null!; // "Consumo", "Ajuste", etc.
    [Required]
    [MaxLength(200)]
    public string? ExternalReference { get; set; } // Pedido, factura, etc.
    [Required]
    [MaxLength(500)]
    public string? Comments { get; set; }

    // Foreign Keys
    public long CreditoId { get; set; }
    public CreditAccount CreditAccount { get; set; } = null!;
}
