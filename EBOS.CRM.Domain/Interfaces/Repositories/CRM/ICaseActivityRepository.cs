using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICaseActivityRepository : IPagedRepository<CaseActivity>, IUnitOfWork
{
    Task<bool> HasOpenByCaseIdAsync(long caseId, CancellationToken cancellationToken = default);
}
