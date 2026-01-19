using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("Credito,CRM")]
public class Credito : ErasableEntity
{
    public long ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public decimal ImporteMaximo { get; set; }      // Límite concedido
    public decimal ImporteConsumido { get; set; }   // Lo ya gastado

    public decimal ImporteDisponible => ImporteMaximo - ImporteConsumido;

    public ICollection<MovimientoCredito> Movimientos { get; set; } = new List<MovimientoCredito>();
}
