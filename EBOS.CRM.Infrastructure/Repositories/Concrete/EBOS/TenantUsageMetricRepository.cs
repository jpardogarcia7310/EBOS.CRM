using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;

public class TenantUsageMetricRepository(CrmDbContext context)
    : BaseRepository<TenantUsageMetric>(context), ITenantUsageMetricRepository;
