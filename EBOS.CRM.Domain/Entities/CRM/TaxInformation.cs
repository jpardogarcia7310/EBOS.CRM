using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.CRM;

public class TaxInformation : ErasableEntity
{
    public string TaxName { get; set; } = null!;
    public string TaxIdentificationNumber { get; set; } = null!;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<TaxInformationAddress> TaxInformationAddresses { get; set; } = new List<TaxInformationAddress>();
}

