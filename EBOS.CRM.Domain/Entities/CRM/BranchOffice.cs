using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.CRM;

public class BranchOffice : ErasableEntity
{
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

