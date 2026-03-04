using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.EBOS;

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
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<ChannelCountry> ChannelCountries { get; set; } = new List<ChannelCountry>();
}
