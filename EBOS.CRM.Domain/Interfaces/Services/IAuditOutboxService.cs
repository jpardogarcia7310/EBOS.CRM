using EBOS.CRM.Contracts.Requests.Services;

namespace EBOS.CRM.Domain.Interfaces.Services;

public interface IAuditOutboxService
{
    Task EnqueueAsync(string operation, AuditInsertRequest request, string? error, CancellationToken cancellationToken = default);
    Task<int> DispatchPendingAsync(CancellationToken cancellationToken = default);
}
