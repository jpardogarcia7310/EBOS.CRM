using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class BranchOffice : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public long CorporateCustomerId { get; set; }
    public CorporateCustomer CorporateCustomer { get; set; } = null!;

    public ICollection<BranchOfficeAddress> BranchOfficeAddresses { get; set; } = new List<BranchOfficeAddress>();
}

