using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("TaxAddresses, CRM")]
public class TaxAddress : ErasableEntity
{
    public string Street { get; set; } = string.Empty;
    public string ExternalNumber { get; set; } = string.Empty;
    public string InternalNumber { get; set; } = string.Empty;
    public string BetweenStreet { get; set; } = string.Empty;
    public string AndStreet { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Municipality { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public long CountryId { get; set; }
    public Country Country { get; set; } = null!;
}