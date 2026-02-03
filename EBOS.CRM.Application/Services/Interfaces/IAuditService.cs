using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Application.Services.Audit;


namespace EBOS.CRM.Application.Services.Interfaces;

public interface IAuditService
{
    Task<AuditInsertResponse> InsertAuditAsync(
        AuditInsertRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditRecord>> GetAllByEntityAsync(
        string entity,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditRecord>> GetAllByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditRecord>> GetAllByRegisterIdAsync(
        long registerId,
        CancellationToken cancellationToken = default);
}


