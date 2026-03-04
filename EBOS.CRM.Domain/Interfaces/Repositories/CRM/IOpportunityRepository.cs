using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface IOpportunityRepository : IPagedRepository<Opportunity>, IUnitOfWork
{
    Task<IReadOnlyCollection<Opportunity>> GetByForecastCriteriaAsync(long tenantId, long? ownerUserId, long? stageId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);


}
