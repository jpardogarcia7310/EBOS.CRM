using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICaseWorkflowService
{
    Task EnsureCanTransitionAsync(Case entity, string nextStatus, CancellationToken cancellationToken = default);
    Task ApplyStatusChangeAsync(Case entity, string nextStatus, DateTime timestamp, CancellationToken cancellationToken = default);
}