using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;

public class TenantQuotaRepository(CrmDbContext context)
    : BaseRepository<TenantQuota>(context), ITenantQuotaRepository;
