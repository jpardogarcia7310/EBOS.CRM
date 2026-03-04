using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IAccountContactRolePrimaryGuard
{
    Task<IReadOnlyCollection<AccountContactRole>> GetOtherPrimariesAsync(long tenantId,
        long accountContactId, long? excludeAccountContactRoleId, CancellationToken cancellationToken = default);
}
