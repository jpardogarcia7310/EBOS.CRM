using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.AddAccountContact;

public class AddAccountContactCommandHandler(
    IAccountContactRepository repository,
    ICorporateCustomerRepository corporateCustomerRepository,
    IIndividualCustomerRepository individualCustomerRepository,
    IAccountContactPrimaryGuard primaryGuard,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null)
    : IRequestHandler<AddAccountContactCommand, AccountContactResponse>
{
    public async Task<AccountContactResponse> Handle(AddAccountContactCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRequest));

        var corporateCustomer = await corporateCustomerRepository.GetByIdAsync(entityRequest.CorporateCustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Corporate customer not found.");
        if (corporateCustomer.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Corporate customer tenant mismatch.");
        }

        var individualCustomer = await individualCustomerRepository.GetByIdAsync(entityRequest.IndividualCustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Individual customer not found.");
        if (individualCustomer.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Individual customer tenant mismatch.");
        }

        var entity = global::EBOS.CRM.Domain.Entities.CRM.AccountContact.Create(
            entityRequest.TenantId,
            entityRequest.CorporateCustomerId,
            entityRequest.IndividualCustomerId,
            entityRequest.IsPrimary,
            entityRequest.StartAt,
            entityRequest.EndAt,
            currentUser.UserId);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            if (entity.IsPrimary)
            {
                var existing = await primaryGuard.GetOtherPrimariesAsync(entityRequest.TenantId,
                    entity.CorporateCustomerId, null, cancellationToken);
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

            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.AccountContact),
                RegisterId: entity.Id,
                OldValues: null,
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
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return mapper.Map<AccountContactResponse>(entity);
    }
}
