using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.Identity;

public class PolicyRole : ErasableEntity
{
    public long PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    public long RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public DateTime AssignedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}
