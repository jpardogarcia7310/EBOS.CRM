using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IAccountContactRoleReferenceValidationService
{
    Task<AccountContact> EnsureAccountContactAvailableAsync(long tenantId, long accountContactId, CancellationToken cancellationToken = default);
}
