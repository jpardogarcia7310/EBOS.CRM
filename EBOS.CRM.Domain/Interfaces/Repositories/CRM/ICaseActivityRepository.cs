using System.Threading;
using System.Threading.Tasks;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICaseActivityRepository
{
    Task<bool> HasOpenByCaseIdAsync(long caseId, CancellationToken cancellationToken = default);
}
