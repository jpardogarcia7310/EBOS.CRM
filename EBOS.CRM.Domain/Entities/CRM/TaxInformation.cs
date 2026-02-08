using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces;

namespace EBOS.CRM.Domain.Entities.CRM;

public class TaxInformation : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string TaxName { get; set; } = null!;
    public string TaxIdentificationNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<TaxInformationAddress> TaxInformationAddresses { get; set; } = new List<TaxInformationAddress>();
}

