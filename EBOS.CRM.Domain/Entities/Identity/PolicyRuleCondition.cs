using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.Identity;

public class PolicyRuleCondition : ErasableEntity
{
    public long PolicyRuleId { get; set; }
    public PolicyRule PolicyRule { get; set; } = null!;

    public long AttributeId { get; set; }
    public AbacAttribute Attribute { get; set; } = null!;

    public string Operator { get; set; } = null!; // "Equals", "Contains", "In", "GreaterThan", etc.
    public string Value { get; set; } = null!;
    public string ValueType { get; set; } = null!; // "String", "Number", "Boolean", "DateTime"
    public bool IsNegated { get; set; }

    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}
