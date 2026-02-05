using System;
using System.Collections.Generic;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.Identity;

public class PolicyRule : ErasableEntity
{
    public long PolicyId { get; set; }
    public Policy Policy { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string Effect { get; set; } = null!; // "Permit" | "Deny"
    public int Priority { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<PolicyRuleCondition> Conditions { get; set; } = new List<PolicyRuleCondition>();
}
