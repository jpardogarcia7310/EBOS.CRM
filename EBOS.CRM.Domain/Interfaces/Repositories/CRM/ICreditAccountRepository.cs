using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface ICreditAccountRepository : IRepository<CreditAccount>, IPagedRepository<CreditAccount>, IUnitOfWork;
