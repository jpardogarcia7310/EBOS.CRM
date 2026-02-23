using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.EBOS;

public class ChannelCountry : BaseEntity
{
    public long ChannelTypeId { get; set; }
    public ChannelType ChannelType { get; set; } = null!;
    public long CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long UpdatedBy { get; set; }
}
