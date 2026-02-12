using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class TaxInformationAddress : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public long TaxInformationId { get; set; }
    public TaxInformation TaxInformation { get; set; } = null!;

    public long AddressId { get; set; }
    public Address Address { get; set; } = null!;

    public bool IsPrimary { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}

