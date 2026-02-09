using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.EBOS;

public class TenantConfigurationRepository(CrmDbContext context)
    : BaseRepository<TenantConfiguration>(context), ITenantConfigurationRepository;
