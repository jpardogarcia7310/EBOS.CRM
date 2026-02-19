using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class OpportunityRepository(CrmDbContext context) : BaseRepository<Opportunity>(context), IOpportunityRepository
{
}
