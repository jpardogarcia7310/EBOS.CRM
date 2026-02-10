using System;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public class IdentificationType : ErasableEntity
{
    public string Code { get; set; } = null!;        // Ej: DNI, NIF, CIF
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}


