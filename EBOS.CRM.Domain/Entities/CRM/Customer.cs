using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Customer : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public long StatusId { get; set; }
    public Status Status { get; set; } = null!;

    public CreditAccount? CreditAccount { get; set; }
    public TaxInformation? TaxInformation { get; set; }
    public BankInformation? BankInformation { get; set; }

    // Optional: Only if you want direct navigation
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
}

