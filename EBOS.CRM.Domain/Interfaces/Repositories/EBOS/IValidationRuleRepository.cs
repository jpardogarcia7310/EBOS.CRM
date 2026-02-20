using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

public interface IValidationRuleRepository : IReadOnlyPagedRepository<ValidationRule>
{
    Task<IReadOnlyCollection<ValidationRule>> GetByKeysAsync(IReadOnlyCollection<string> keys,
        CancellationToken cancellationToken = default);
}
