using System;
using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Sla : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Name { get; set; } = null!;
    public int TargetMinutes { get; set; }
    public int? WarningMinutes { get; set; }
    public DateTime? ActiveFrom { get; set; }
    public DateTime? ActiveTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<Case> Cases { get; set; } = new List<Case>();

    public bool IsActiveAt(DateTime date)
    {
        if (!IsActive)
        {
            return false;
        }

        if (ActiveFrom.HasValue && date < ActiveFrom.Value)
        {
            return false;
        }

        if (ActiveTo.HasValue && date > ActiveTo.Value)
        {
            return false;
        }

        return true;
    }

    public DateTime CalculateDueAt(DateTime start)
    {
        if (TargetMinutes <= 0)
        {
            throw new InvalidOperationException("TargetMinutes must be greater than zero.");
        }

        return start.AddMinutes(TargetMinutes);
    }

    public bool IsBreached(DateTime now, DateTime? dueAt)
    {
        if (!dueAt.HasValue)
        {
            return false;
        }

        return now > dueAt.Value;
    }
}
