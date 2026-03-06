using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.UpdateAccountContact;

public class UpdateAccountContactCommandHandler(
    IAccountContactRepository repository,
    ICorporateCustomerRepository corporateCustomerRepository,
    IIndividualCustomerRepository individualCustomerRepository,
    IAccountContactPrimaryGuard primaryGuard,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null)
    : IRequestHandler<UpdateAccountContactCommand, AccountContactResponse?>
{
    public async Task<AccountContactResponse?> Handle(UpdateAccountContactCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRequest));

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;

        var corporateCustomer = await corporateCustomerRepository.GetByIdAsync(entityRequest.CorporateCustomerId, cancellationToken)
            ?? throw new DomainValidationException("Corporate customer not found.", "DOMAIN_VALIDATION_CORPORATE_CUSTOMER_NOT_FOUND");
        if (corporateCustomer.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Corporate customer tenant mismatch.", "DOMAIN_CONFLICT_CORPORATE_CUSTOMER_TENANT_MISMATCH");
        }

        var individualCustomer = await individualCustomerRepository.GetByIdAsync(entityRequest.IndividualCustomerId, cancellationToken)
            ?? throw new DomainValidationException("Individual customer not found.", "DOMAIN_VALIDATION_INDIVIDUAL_CUSTOMER_NOT_FOUND");
        if (individualCustomer.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Individual customer tenant mismatch.", "DOMAIN_CONFLICT_INDIVIDUAL_CUSTOMER_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.Assign(entityRequest.CorporateCustomerId, entityRequest.IndividualCustomerId, entityRequest.StartAt);
        if (entityRequest.EndAt.HasValue)
        {
            entity.Unassign(entityRequest.EndAt.Value);
        }
        entity.SetPrimary(entityRequest.IsPrimary);
        entity.Touch(currentUser.UserId);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            if (entity.IsPrimary)
            {
                var existing = await primaryGuard.GetOtherPrimariesAsync(entityRequest.TenantId,
                    entity.CorporateCustomerId, entity.Id, cancellationToken);
                foreach (var contact in existing)
                {
                    contact.SetPrimary(false);
                    contact.Touch(currentUser.UserId);
                    await repository.UpdateAsync(contact, cancellationToken);
                    if (domainOperationalEventPublisher is not null)
                    {
                        await domainOperationalEventPublisher.PublishAsync(
                            nameof(Domain.Entities.CRM.AccountContact),
                            contact.Id,
                            contact.DequeueOperationalEvents(),
                            cancellationToken);
                    }
                }
            }

            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.AccountContact),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.AccountContact),
                    entity.Id,
                    entity.DequeueOperationalEvents(),
                    cancellationToken);
            }
            await repository.CommitAsync(cancellationToken);
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

        return mapper.Map<AccountContactResponse>(entity);
    }
}

