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

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;

public class SetPrimaryAccountContactCommandHandler(
    IAccountContactRepository repository,
    IAccountContactPrimaryGuard primaryGuard,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null)
    : IRequestHandler<SetPrimaryAccountContactCommand, AccountContactResponse?>
{
    public async Task<AccountContactResponse?> Handle(SetPrimaryAccountContactCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRequest));

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;

        if (entity.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Account contact tenant mismatch.", "DOMAIN_CONFLICT_ACCOUNT_CONTACT_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
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
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return mapper.Map<AccountContactResponse>(entity);
    }
}
