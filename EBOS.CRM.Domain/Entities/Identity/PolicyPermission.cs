using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.Identity;

public class PolicyPermission : ErasableEntity
{
    public long PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    public long PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    public DateTime AssignedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}
