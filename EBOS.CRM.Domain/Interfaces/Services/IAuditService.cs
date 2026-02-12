using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Services.Models;

namespace EBOS.CRM.Domain.Interfaces.Services;

public interface IAuditService
{
    Task<AuditInsertResponse> InsertAuditAsync(AuditInsertRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditRecord>> GetAllByEntityAsync(string entity,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditRecord>> GetAllByUserIdAsync(long userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditRecord>> GetAllByRegisterIdAsync(long registerId,
        CancellationToken cancellationToken = default);
}