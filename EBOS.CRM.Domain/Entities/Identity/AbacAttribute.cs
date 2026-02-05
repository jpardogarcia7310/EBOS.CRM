using System;
using System.Collections.Generic;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.Identity;

public class AbacAttribute : ErasableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!; // "Subject", "Resource", "Action", "Environment"
    public string DataType { get; set; } = null!; // "String", "Number", "Boolean", "DateTime"
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<PolicyRuleCondition> PolicyRuleConditions { get; set; } = new List<PolicyRuleCondition>();
}
