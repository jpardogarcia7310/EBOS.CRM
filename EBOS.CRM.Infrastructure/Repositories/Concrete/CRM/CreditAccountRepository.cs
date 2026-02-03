using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CreditAccountRepository(CrmDbContext context) : BaseRepository<CreditAccount>(context),
    ICreditAccountRepository
{
}


