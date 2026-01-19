using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("Paises, EBOS")]
public class Pais : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;
    [Required]
    [MaxLength(2)]
    public string Iso31661A2Code { get; set; } = default!;
    [Required]
    [MaxLength(3)]
    public string Iso31661A3Code { get; set; } = default!;
    [Required]
    [MaxLength(10)]
    public string Iso31661NumCode { get; set; } = default!;
    [Required]
    [MaxLength(5)]
    public string Domain { get; set; } = default!;
    [Required]
    [MaxLength(100)]
    public string Currency { get; set; } = default!;
    [Required]
    [MaxLength(10)]
    public string CurrencyCode { get; set; } = default!;
    [Required]
    [MaxLength(20)]
    public string InternationalPhoneCode { get; set; } = default!;
}