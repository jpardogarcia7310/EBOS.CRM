using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

public interface ITenantQuotaRepository : IPagedRepository<TenantQuota>, IUnitOfWork;
