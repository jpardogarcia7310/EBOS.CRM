using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.UpdateAccountContactRole;

public class UpdateAccountContactRoleCommandHandler(
    IAccountContactRoleRepository repository,
    IAccountContactRepository accountContactRepository,
    IAccountContactRolePrimaryGuard primaryGuard,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null)
    : IRequestHandler<UpdateAccountContactRoleCommand, AccountContactRoleResponse?>
{
    public async Task<AccountContactRoleResponse?> Handle(UpdateAccountContactRoleCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRoleRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRoleRequest));

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;

        var accountContact = await accountContactRepository.GetByIdAsync(entityRequest.AccountContactId, cancellationToken)
            ?? throw new InvalidOperationException("Account contact not found.");
        if (accountContact.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Account contact tenant mismatch.");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.Update(
            entityRequest.TenantId,
            entityRequest.AccountContactId,
            entityRequest.RoleCode,
            entityRequest.IsPrimary,
            entityRequest.ValidFrom,
            entityRequest.ValidTo);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            if (entity.IsPrimary)
            {
                var existing = await primaryGuard.GetOtherPrimariesAsync(entityRequest.TenantId,
                    entity.AccountContactId, entity.Id, cancellationToken);
                foreach (var role in existing)
                {
                    role.SetPrimary(false);
                    await repository.UpdateAsync(role, cancellationToken);
                    if (domainOperationalEventPublisher is not null)
                    {
                        await domainOperationalEventPublisher.PublishAsync(
                            nameof(Domain.Entities.CRM.AccountContactRole),
                            role.Id,
                            role.DequeueOperationalEvents(),
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
                Entity: nameof(Domain.Entities.CRM.AccountContactRole),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.AccountContactRole),
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

        return mapper.Map<AccountContactRoleResponse>(entity);
    }
}
