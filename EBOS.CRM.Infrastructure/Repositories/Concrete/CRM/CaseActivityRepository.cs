using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CaseActivityRepository : ICaseActivityRepository
{
    public Task<bool> HasOpenByCaseIdAsync(long caseId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}
