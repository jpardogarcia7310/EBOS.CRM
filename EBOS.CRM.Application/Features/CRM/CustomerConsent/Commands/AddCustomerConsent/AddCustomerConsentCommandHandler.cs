using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;

public class AddCustomerConsentCommandHandler(
    ICustomerConsentRepository repository,
    ICustomerRepository customerRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    ICustomer360Metrics metrics,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null)
    : IRequestHandler<AddCustomerConsentCommand, CustomerConsentResponse>
{
    public async Task<CustomerConsentResponse> Handle(AddCustomerConsentCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.ConsentRequest ??
                            throw new ArgumentNullException(nameof(request.ConsentRequest));

        var customer = await customerRepository.GetByIdAsync(entityRequest.CustomerId, cancellationToken)
            ?? throw new DomainValidationException("Customer not found.", "DOMAIN_VALIDATION_CUSTOMER_NOT_FOUND");
        if (customer.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Customer tenant mismatch.", "DOMAIN_CONFLICT_CUSTOMER_TENANT_MISMATCH");
        }

        var entity = entityRequest.Granted
            ? global::EBOS.CRM.Domain.Entities.CRM.CustomerConsent.Create(
                entityRequest.TenantId,
                entityRequest.CustomerId,
                entityRequest.ConsentType,
                entityRequest.Granted,
                entityRequest.GrantedAt,
                entityRequest.Source,
                entityRequest.ExpiresAt)
            : global::EBOS.CRM.Domain.Entities.CRM.CustomerConsent.CreateRevoked(
                entityRequest.TenantId,
                entityRequest.CustomerId,
                entityRequest.ConsentType,
                entityRequest.ExpiresAt ?? entityRequest.GrantedAt,
                entityRequest.Source,
                entityRequest.ExpiresAt);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.CustomerConsent),
                RegisterId: entity.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.CustomerConsent),
                    entity.Id,
                    entity.DequeueOperationalEvents(),
                    cancellationToken);
            }
            await repository.CommitAsync(cancellationToken);
            metrics.RecordConsentEvent(entity.TenantId, entity.ConsentType, entity.Granted);
        }
        catch (Exception ex)
        {
            await repository.RollbackAsync(cancellationToken);

            if (DomainTransientFailureClassifier.TryClassify(ex, nameof(Handle), out var transient))
            {
                throw transient;
            }

            throw;
        }

        return mapper.Map<CustomerConsentResponse>(entity);
    }
}

