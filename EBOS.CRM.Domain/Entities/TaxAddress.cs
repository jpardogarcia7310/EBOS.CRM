using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("TaxAddresses, CRM")]
public class TaxAddress : ErasableEntity
{
    [Required]
    [MaxLength(255)]
    public string Street { get; set; } = string.Empty;
    [Required]
    [MaxLength(10)]
    public string ExternalNumber { get; set; } = string.Empty;
    [Required]
    [MaxLength(10)]
    public string InternalNumber { get; set; } = string.Empty;
    [Required]
    [MaxLength(10)]
    public string PostalCode { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string State { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Municipality { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Neighborhood { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Reference { get; set; } = string.Empty;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public long CountryId { get; set; }
    public Country Country { get; set; } = null!;
}