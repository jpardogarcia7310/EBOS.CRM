using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CaseActivity : ErasableEntity, ITenantScopedEntity
{
    public const string StatusOpen = "Open";
    public const string StatusInProgress = "InProgress";
    public const string StatusCompleted = "Completed";
    public const string StatusCancelled = "Cancelled";

    public long TenantId { get; set; }
    public long CaseId { get; set; }
    public Case Case { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public void SetStatus(string status)
    {
        if (!IsValidStatus(status))
        {
            throw new InvalidOperationException("Status value is invalid.");
        }

        if (!IsValidTransition(Status, status))
        {
            throw new InvalidOperationException("Status transition is not allowed.");
        }

        Status = status;
    }

    public static bool IsValidStatus(string status)
    {
        return status is StatusOpen or StatusInProgress or StatusCompleted or StatusCancelled;
    }

    public static bool IsValidTransition(string currentStatus, string nextStatus)
    {
        if (string.IsNullOrWhiteSpace(currentStatus))
        {
            return nextStatus == StatusOpen;
        }

        return currentStatus switch
        {
            StatusOpen => nextStatus is StatusInProgress or StatusCompleted or StatusCancelled,
            StatusInProgress => nextStatus is StatusCompleted or StatusCancelled,
            StatusCompleted => false,
            StatusCancelled => false,
            _ => false
        };
    }
}
