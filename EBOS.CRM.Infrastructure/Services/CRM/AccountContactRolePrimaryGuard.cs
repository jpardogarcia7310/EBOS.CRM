using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Infrastructure.Services.CRM;

public class AccountContactRolePrimaryGuard(IAccountContactRoleRepository repository)
    : IAccountContactRolePrimaryGuard
{
    public async Task<IReadOnlyCollection<AccountContactRole>> GetOtherPrimariesAsync(long tenantId,
        long accountContactId, long? excludeAccountContactRoleId, CancellationToken cancellationToken = default)
    {
        var roles = await repository.GetByAccountContactPagedAsync(tenantId, accountContactId, 1, int.MaxValue,
            cancellationToken);

        return roles
            .Where(role => role.IsPrimary)
            .Where(role => !excludeAccountContactRoleId.HasValue || role.Id != excludeAccountContactRoleId.Value)
            .ToList();
    }
}
