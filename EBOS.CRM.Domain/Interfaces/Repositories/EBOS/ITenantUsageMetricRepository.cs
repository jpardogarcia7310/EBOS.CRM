using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

public interface ITenantUsageMetricRepository : IPagedRepository<TenantUsageMetric>, IUnitOfWork;
