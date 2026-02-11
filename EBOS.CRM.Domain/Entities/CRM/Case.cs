using System;
using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Case : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public long OwnerUserId { get; set; }
    public long QueueId { get; set; }
    public Queue Queue { get; set; } = null!;
    public long SlaId { get; set; }
    public Sla Sla { get; set; } = null!;
    public DateTime? DueAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public void AssignQueue(long queueId)
    {
        if (queueId <= 0)
        {
            throw new InvalidOperationException("QueueId must be a positive value.");
        }

        QueueId = queueId;
    }

    public void AssignOwner(long ownerUserId)
    {
        if (ownerUserId <= 0)
        {
            throw new InvalidOperationException("OwnerUserId must be a positive value.");
        }

        OwnerUserId = ownerUserId;
    }

    public void Close(DateTime closedAt)
    {
        if (ClosedAt.HasValue)
        {
            throw new InvalidOperationException("Case is already closed.");
        }

        ClosedAt = closedAt;
    }

    public void Reopen()
    {
        if (!ClosedAt.HasValue)
        {
            throw new InvalidOperationException("Case is not closed.");
        }

        ClosedAt = null;
    }

    public void UpdateDueAt(DateTime? dueAt)
    {
        DueAt = dueAt;
    }
}
