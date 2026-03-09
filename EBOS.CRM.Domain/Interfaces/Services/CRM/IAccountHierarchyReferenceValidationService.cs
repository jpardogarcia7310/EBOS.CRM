using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IAccountHierarchyReferenceValidationService
{
    Task<CorporateCustomer> EnsureCorporateCustomerAvailableAsync(long tenantId, long corporateCustomerId, string role, CancellationToken cancellationToken = default);
}
