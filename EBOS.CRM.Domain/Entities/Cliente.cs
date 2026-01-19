using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("Clientes,CRM")]
public abstract class Cliente : ErasableEntity
{
    [Required]
    [MaxLength(50)]
    public string Codigo { get; set; } = null!; // Código interno CRM
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = null!;
    [Required]
    [MaxLength(12)]
    public string Telefono { get; set; } = null!;

    public long? DatosFiscalesId { get; set; }
    public DatosFiscales? DatosFiscales { get; set; }

    public long? DatosBancariosId { get; set; }
    public DatosBancarios? DatosBancarios { get; set; }

    public Credito? Credito { get; set; }

    public DateTime FechaAlta { get; set; }

    public long EstadoId { get; set; }
    public Estado Estado { get; set; } 

    public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
}
