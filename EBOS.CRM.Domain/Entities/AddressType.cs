using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Entities.CRM;


namespace EBOS.CRM.Domain.Entities;

public class AddressType : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;

    // New: Enterprise semantics
    public string? Category { get; set; } // "Shipping", "Billing", "Fiscal", "Operational", etc.
    public bool AllowsMultiple { get; set; }
    public bool RequiresPrimary { get; set; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}

