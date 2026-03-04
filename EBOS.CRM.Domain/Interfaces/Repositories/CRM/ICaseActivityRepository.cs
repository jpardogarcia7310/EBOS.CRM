using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICaseActivityRepository : IPagedRepository<CaseActivity>, IUnitOfWork
{
    Task<bool> HasOpenByCaseIdAsync(long caseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CaseActivity>> GetAllByCaseIdAsync(long caseId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CaseActivity>> GetAllByCaseIdPagedAsync(long caseId, int pageNumber, int pageSize,
        string? status = null, DateTime? from = null, DateTime? to = null,
        CancellationToken cancellationToken = default);
    Task<int> CountByCaseIdAsync(long caseId, CancellationToken cancellationToken = default);
    Task<int> CountByCaseIdAsync(long caseId, string? status = null, DateTime? from = null, DateTime? to = null,
        CancellationToken cancellationToken = default);
}
