using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.Identity;

public class Permission : ErasableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<PolicyPermission> PolicyPermissions { get; set; } = new List<PolicyPermission>();
}
