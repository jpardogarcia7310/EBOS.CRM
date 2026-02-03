using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Address : ErasableEntity
{
    public string Street { get; set; } = null!;
    public string ExternalNumber { get; set; } = null!;
    public string? InternalNumber { get; set; }
    public string? BetweenStreet1 { get; set; }
    public string? BetweenStreet2 { get; set; }
    public string? Neighbourhood { get; set; }
    public string City { get; set; } = null!;
    public string StateOrProvince { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string? GoogleMapsUrl { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public long CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public long AddressTypeId { get; set; }
    public AddressType AddressType { get; set; } = null!;

    public ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
    public ICollection<BranchOfficeAddress> BranchOfficeAddresses { get; set; } = new List<BranchOfficeAddress>();
    public ICollection<TaxInformationAddress> TaxInformationAddresses { get; set; } = new List<TaxInformationAddress>();
}

