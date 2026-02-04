using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.Identity;

public class Policy : ErasableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<UserPolicy> UserPolicies { get; set; } = new List<UserPolicy>();
    public ICollection<PolicyPermission> PolicyPermissions { get; set; } = new List<PolicyPermission>();
}
