using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;

namespace EBOS.CRM.Domain.Entities;

public class Country : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Iso31661A2Code { get; set; } = null!;
    public string Iso31661A3Code { get; set; } = null!;
    public string Iso31661NumCode { get; set; } = null!;
    public string Domain { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string CurrencyCode { get; set; } = null!;
    public string InternationalPhoneCode { get; set; } = null!;
}