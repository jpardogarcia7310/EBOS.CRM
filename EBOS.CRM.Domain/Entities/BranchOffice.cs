using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public class BranchOffice : ErasableEntity
{
    public long CorporateCustomerId { get; set; }
    public CorporateCustomer CorporateCustomer { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    [Required]
    [MaxLength(300)]
    public string AddressLine { get; set; } = null!;
    [Required]
    [MaxLength(150)]
    public string City { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    // Foreign Keys
    public long CountryId { get; set; }
    public Country Country { get; set; } = null!;
}
