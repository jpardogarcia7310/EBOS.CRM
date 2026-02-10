using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete;

public class IdentificationTypeRepository(CrmDbContext context) : BaseRepository<IdentificationType>(context), 
    IIdentificationTypeRepository 
{ }
