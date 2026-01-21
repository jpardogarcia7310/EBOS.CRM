using System.ComponentModel.DataAnnotations;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public class IdentificationType : ErasableEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!;        // Ej: DNI, NIF, CIF
    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = null!; // Ej: Documento Nacional de Identidad
}
