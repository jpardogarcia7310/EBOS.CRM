using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("TaxInformation, CRM")]
public class TaxInformation : ErasableEntity
{
    [Required]
    [MaxLength(200)]
    public string TaxName { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string TaxIdentificationNumber { get; set; } = null!;
    [Required]
    [MaxLength(300)]
    public string FiscalAddress { get; set; } = null!;
    [Required]
    [MaxLength(150)]
    public string City { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = null!;
    
    // Foreign Keys
    public long CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public Customer Customer { get; set; } = null!;
}
