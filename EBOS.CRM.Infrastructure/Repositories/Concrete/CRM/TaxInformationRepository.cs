using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Persistence;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class TaxInformationRepository(CrmDbContext context) : BaseRepository<TaxInformation>(context), 
    ITaxInformationRepository
{
}