using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public sealed class BranchOffice : ErasableEntity
{
    public long CorporateCustomerId { get; set; }
    public CorporateCustomer CorporateCustomer { get; set; } = null!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    // Foreign Keys
    public long AddressId { get; set; }
    [ForeignKey(nameof(AddressId))]
    public Address Address { get; set; } = null!;
}
