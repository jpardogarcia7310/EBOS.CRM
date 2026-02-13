using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.EBOS;

public class ChannelType : BaseEntity
{
    public string Descripcion { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long UpdatedBy { get; set; }

    public ICollection<CRM.CustomerPreference> CustomerPreferences { get; set; } = new List<CRM.CustomerPreference>();
}
