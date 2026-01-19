using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("MovimientoCredito,CRM")]
public class MovimientoCredito : ErasableEntity
{
    public long CreditoId { get; set; }
    public Credito Credito { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public decimal Importe { get; set; } // Positivo: consumo, Negativo: devolución/ajuste
    public string Tipo { get; set; } = null!; // "Consumo", "Ajuste", etc.
    public string? ReferenciaExterna { get; set; } // Pedido, factura, etc.
    public string? Comentarios { get; set; }
}
