using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IAccountContactPrimaryGuard
{
    Task<IReadOnlyCollection<AccountContact>> GetOtherPrimariesAsync(long tenantId, long corporateCustomerId,
        long? excludeAccountContactId, CancellationToken cancellationToken = default);
}
