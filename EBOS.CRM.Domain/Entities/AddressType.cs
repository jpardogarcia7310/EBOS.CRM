using System.ComponentModel.DataAnnotations;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public class AddressType : ErasableEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!;        // Ej: MAIN, BILLING, SHIPPING
    [Required]
    [MaxLength(50)]
    public string Description { get; set; } = null!; // Ej: Dirección principal
}
