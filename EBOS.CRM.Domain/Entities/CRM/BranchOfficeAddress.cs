using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.CRM;

public class BranchOfficeAddress : ErasableEntity
{
    public long BranchOfficeId { get; set; }
    public BranchOffice BranchOffice { get; set; } = null!;

    public long AddressId { get; set; }
    public Address Address { get; set; } = null!;

    public bool IsPrimary { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsCurrent { get; set; }
}


