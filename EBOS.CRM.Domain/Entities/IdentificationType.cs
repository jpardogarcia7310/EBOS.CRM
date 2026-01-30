using System.ComponentModel.DataAnnotations;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public class IdentificationType : ErasableEntity
{
    public string Code { get; set; } = null!;        // Ej: DNI, NIF, CIF
    public string Description { get; set; } = null!; 
}
