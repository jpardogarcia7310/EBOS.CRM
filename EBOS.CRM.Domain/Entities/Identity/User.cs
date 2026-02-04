using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.Identity;

public class User : ErasableEntity
{
    public string ExternalId { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserPolicy> UserPolicies { get; set; } = new List<UserPolicy>();
}
