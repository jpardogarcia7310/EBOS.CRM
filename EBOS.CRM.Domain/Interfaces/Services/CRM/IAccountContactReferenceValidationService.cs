using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IAccountContactReferenceValidationService
{
    Task<CorporateCustomer> EnsureCorporateCustomerAvailableAsync(long tenantId, long corporateCustomerId, CancellationToken cancellationToken = default);
    Task<IndividualCustomer> EnsureIndividualCustomerAvailableAsync(long tenantId, long individualCustomerId, CancellationToken cancellationToken = default);
}
