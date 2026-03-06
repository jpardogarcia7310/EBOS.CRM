using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy;

public sealed class CustomerPrivacyExecutionService(
    ICustomerRepository customerRepository,
    ICorporateCustomerRepository corporateCustomerRepository,
    IIndividualCustomerRepository individualCustomerRepository,
    ICustomerPrivacyRequestRepository privacyRequestRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser)
{
    public async Task ExecuteAsync(CustomerPrivacyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            request.MarkInProgress(currentUser.UserId);
            await privacyRequestRepository.UpdateAsync(request, cancellationToken);
            await privacyRequestRepository.SaveChangesAsync(cancellationToken);

            var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
                           ?? throw new DomainValidationException("Customer not found.", "DOMAIN_VALIDATION_CUSTOMER_NOT_FOUND");
            if (customer.TenantId != request.TenantId)
            {
                throw new DomainConflictException("Customer tenant mismatch.", "DOMAIN_CONFLICT_CUSTOMER_TENANT_MISMATCH");
            }

            var requestOldValues = AuditSerialization.Serialize(request);
            var customerOldValues = AuditSerialization.Serialize(customer);

            if (request.RequestType is CustomerPrivacyRequest.TypeForget or CustomerPrivacyRequest.TypeAnonymize)
            {
                await AnonymizeCustomerAsync(customer, request, cancellationToken);
                await customerRepository.UpdateAsync(customer, cancellationToken);
            }

            request.MarkCompleted(currentUser.UserId);
            await privacyRequestRepository.UpdateAsync(request, cancellationToken);
            await privacyRequestRepository.SaveChangesAsync(cancellationToken);

            await WriteAuditAsync(
                AuditActions.Update,
                nameof(Domain.Entities.CRM.Customer),
                customer.Id,
                customerOldValues,
                AuditSerialization.Serialize(customer),
                cancellationToken);

            await WriteAuditAsync(
                AuditActions.Update,
                nameof(CustomerPrivacyRequest),
                request.Id,
                requestOldValues,
                AuditSerialization.Serialize(request),
                cancellationToken);
        }
        catch (Exception ex)
        {
            request.MarkFailed(currentUser.UserId, "EXECUTION_ERROR", ex.Message);
            await privacyRequestRepository.UpdateAsync(request, cancellationToken);
            await privacyRequestRepository.SaveChangesAsync(cancellationToken);
            if (DomainTransientFailureClassifier.TryClassify(ex, nameof(ExecuteAsync), out var transient))
            {
                request.RecordTransientFailure(transient.Code, nameof(ExecuteAsync));
                throw transient;
            }

            throw;
        }
    }

    private async Task AnonymizeCustomerAsync(global::EBOS.CRM.Domain.Entities.CRM.Customer customer, CustomerPrivacyRequest request,
        CancellationToken cancellationToken)
    {
        customer.Email = BuildAnonymizedEmail(customer.Id, request.Id);
        customer.Phone = BuildAnonymizedPhone(customer.Id);
        customer.Source = "ANONYMIZED";
        customer.Confidentiality = "RESTRICTED";
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedBy = currentUser.UserId;

        var corporate = await corporateCustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
        if (corporate is not null)
        {
            corporate.LegalName = $"ANONYMIZED-CORP-{customer.Id}";
            corporate.TaxIdentification = BuildCorporateTaxId(customer.Id);
            corporate.UpdatedAt = DateTime.UtcNow;
            corporate.UpdatedBy = currentUser.UserId;
            await corporateCustomerRepository.UpdateAsync(corporate, cancellationToken);
            return;
        }

        var individual = await individualCustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
        if (individual is not null)
        {
            individual.FirstName = "ANON";
            individual.LastName = "CUSTOMER";
            individual.IdentificationNumber = BuildIndividualIdNumber(customer.Id);
            individual.BirthDate = new DateTime(1900, 1, 1);
            individual.UpdatedAt = DateTime.UtcNow;
            individual.UpdatedBy = currentUser.UserId;
            await individualCustomerRepository.UpdateAsync(individual, cancellationToken);
        }
    }

    private async Task WriteAuditAsync(string action, string entity, long registerId, string? oldValues, string? newValues,
        CancellationToken cancellationToken)
    {
        var auditRequest = new AuditInsertRequest(
            UserId: currentUser.UserId,
            TimeStamp: DateTimeOffset.UtcNow,
            Action: action,
            Entity: entity,
            RegisterId: registerId,
            OldValues: oldValues,
            NewValues: newValues,
            CorrelationId: currentUser.CorrelationId);

        await auditService.InsertAuditAsync(auditRequest, cancellationToken);
    }

    private static string BuildAnonymizedEmail(long customerId, long requestId)
    {
        return $"anon-{customerId}-{requestId}@redacted.local";
    }

    private static string BuildAnonymizedPhone(long customerId)
    {
        var numeric = (customerId % 1000000000000L).ToString();
        return numeric.PadLeft(12, '9');
    }

    private static string BuildCorporateTaxId(long customerId)
    {
        return $"ANON{customerId % 999999999999999L}";
    }

    private static string BuildIndividualIdNumber(long customerId)
    {
        return $"A{customerId % 999999999}";
    }
}
