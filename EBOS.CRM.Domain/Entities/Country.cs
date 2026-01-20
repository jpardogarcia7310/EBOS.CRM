using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;

namespace EBOS.CRM.Domain.Entities;

public class Country : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    [Required]
    [MaxLength(2)]
    public string Iso31661A2Code { get; set; } = null!;
    [Required]
    [MaxLength(3)]
    public string Iso31661A3Code { get; set; } = null!;
    [Required]
    [MaxLength(10)]
    public string Iso31661NumCode { get; set; } = null!;
    [Required]
    [MaxLength(5)]
    public string Domain { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Currency { get; set; } = null!;
    [Required]
    [MaxLength(10)]
    public string CurrencyCode { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string InternationalPhoneCode { get; set; } = null!; 
}