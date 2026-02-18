namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IAccountHierarchyCycleGuard
{
    Task<bool> CreatesCycleAsync(long tenantId, long parentCorporateCustomerId, long childCorporateCustomerId,
        CancellationToken cancellationToken = default);
}
