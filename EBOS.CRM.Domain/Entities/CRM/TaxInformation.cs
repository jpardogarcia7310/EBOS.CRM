using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.CRM;

public sealed class TaxInformation : ErasableEntity
{
    [Required]
    [MaxLength(200)]
    public string TaxName { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string TaxIdentificationNumber { get; set; } = null!;
     
    // Foreign Keys
    public long AddressId { get; set; }
    [ForeignKey(nameof(AddressId))]
    public Address Address { get; set; } = null!;

    public long CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;
}
