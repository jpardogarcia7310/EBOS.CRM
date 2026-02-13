using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICustomerDedupeRepository
{
    Task<IReadOnlyCollection<CustomerDuplicateCandidate>> FindDuplicatesAsync(CustomerDedupeCriteria criteria,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountDuplicatesAsync(CustomerDedupeCriteria criteria, CancellationToken cancellationToken = default);
}
