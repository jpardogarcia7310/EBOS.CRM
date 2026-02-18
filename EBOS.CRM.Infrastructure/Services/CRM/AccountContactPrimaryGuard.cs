using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Infrastructure.Services.CRM;

public class AccountContactPrimaryGuard(IAccountContactRepository repository) : IAccountContactPrimaryGuard
{
    public async Task<IReadOnlyCollection<AccountContact>> GetOtherPrimariesAsync(long tenantId,
        long corporateCustomerId, long? excludeAccountContactId, CancellationToken cancellationToken = default)
    {
        var contacts = await repository.GetByCorporateCustomerPagedAsync(tenantId, corporateCustomerId, 1,
            int.MaxValue, cancellationToken);

        return contacts
            .Where(contact => contact.IsPrimary)
            .Where(contact => !excludeAccountContactId.HasValue || contact.Id != excludeAccountContactId.Value)
            .ToList();
    }
}
