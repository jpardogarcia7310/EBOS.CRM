using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public class Address : ErasableEntity
{
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = "Principal"; // Fiscal, Comercial, Envío, etc.
    [Required]
    [MaxLength(200)]
    public string Street { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string ExternalNumber { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string? InternalNumber { get; set; }
    [Required]
    [MaxLength(200)]
    public string? BetweenStreet1 { get; set; }
    [Required]
    [MaxLength(200)]
    public string? BetweenStreet2 { get; set; }
    [Required]
    [MaxLength(200)]
    public string? Neighborhood { get; set; }
    [Required]
    [MaxLength(150)]
    public string City { get; set; } = null!;
    [Required]
    [MaxLength(150)]
    public string StateOrProvince { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = null!;
    [Required]
    [MaxLength(500)]
    public string? GoogleMapsUrl { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Foreign Keys
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public long CountryId { get; set; }
    public Country Country { get; set; } = null!;
}
