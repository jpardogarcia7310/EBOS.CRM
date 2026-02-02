using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM;

public interface IBankInformationRepository : IRepository<BankInformation>, IPagedRepository<BankInformation>, IUnitOfWork;
