using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.EBOS;

public class ValidationRule : ErasableEntity
{
    public string Key { get; set; } = null!; // e.g. "postal_code:EC", "tax_id:DEFAULT", "id:DNI"
    public string Pattern { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}
