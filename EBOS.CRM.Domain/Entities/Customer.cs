using EBOS.Core.Primitives;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("Customers, CRM")]
public class Customer : ErasableEntity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    [Range(0, 9999999999999999.99)]
    public decimal Balance { get; set; }
    [Required]
    [DefaultValue(false)]
    public bool IsCompany { get; set; }
    public bool? CompanyType { get; set; } // True = HeadOffice, False = Branch
    [Required]
    [MaxLength(50)]
    public string RFC { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string CURP { get; set; } = string.Empty;
    [Required]
    [DefaultValue(false)]
    public bool TaxDuplicateShippingAddress { get; set; }

    [Required]
    public long StatusId { get; set; }
    public Status Status { get; set; } = default!;
    [Required]
    public long TaxRegimeId { get; set; }
    public TaxRegime TaxRegime { get; set; } = default!;
    [Required]
    public long TaxAddressId { get; set; } = default!;
    public TaxAddress TaxAddress { get; set; } = default!;
    public long? ShippingAddressId { get; set; }
    public ShippingAddress? ShippingAddress { get; set; }
    [Required]
    public long SalesConfigurationId { get; set; }
    public SalesData SalesConfiguration { get; set; } = default!;
    [Required]
    public long CustomerHistoryId { get; set; }
    public CustomerHistory CustomerHistory { get; set; } = default!;
}
