using System;
using System.Collections.Generic;
using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Address : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public long CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public long AddressTypeId { get; set; }
    public AddressType AddressType { get; set; } = null!;

    public ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
    public ICollection<BranchOfficeAddress> BranchOfficeAddresses { get; set; } = new List<BranchOfficeAddress>();
    public ICollection<TaxInformationAddress> TaxInformationAddresses { get; set; } = new List<TaxInformationAddress>();
}

