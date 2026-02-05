using System;
using System.Collections.Generic;
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
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<UserPolicy> UserPolicies { get; set; } = new List<UserPolicy>();
    public ICollection<PolicyPermission> PolicyPermissions { get; set; } = new List<PolicyPermission>();
    public ICollection<PolicyRole> PolicyRoles { get; set; } = new List<PolicyRole>();
    public ICollection<PolicyRule> PolicyRules { get; set; } = new List<PolicyRule>();
}
