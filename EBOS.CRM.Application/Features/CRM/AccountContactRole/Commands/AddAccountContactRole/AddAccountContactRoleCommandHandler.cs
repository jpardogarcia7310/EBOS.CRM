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

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;

public class AddAccountContactRoleCommandHandler(
    IAccountContactRoleRepository repository,
    IAccountContactRepository accountContactRepository,
    IAccountContactRolePrimaryGuard primaryGuard,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IAccountContactRoleReferenceValidationService? accountContactRoleReferenceValidationService = null,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null)
    : IRequestHandler<AddAccountContactRoleCommand, AccountContactRoleResponse>
{
    public async Task<AccountContactRoleResponse> Handle(AddAccountContactRoleCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRoleRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRoleRequest));

        if (accountContactRoleReferenceValidationService is null)
        {
            var accountContact = await accountContactRepository.GetByIdAsync(entityRequest.AccountContactId, cancellationToken)
                ?? throw new DomainValidationException("Account contact not found.", "DOMAIN_VALIDATION_ACCOUNT_CONTACT_NOT_FOUND");
            if (accountContact.TenantId != entityRequest.TenantId)
            {
                throw new DomainConflictException("Account contact tenant mismatch.", "DOMAIN_CONFLICT_ACCOUNT_CONTACT_TENANT_MISMATCH");
            }
        }
        else
        {
            _ = await accountContactRoleReferenceValidationService.EnsureAccountContactAvailableAsync(entityRequest.TenantId, entityRequest.AccountContactId, cancellationToken);
        }

        var entity = global::EBOS.CRM.Domain.Entities.CRM.AccountContactRole.Create(
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
                    entity.AccountContactId, null, cancellationToken);
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

            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.AccountContactRole),
                RegisterId: entity.Id,
                OldValues: null,
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
        catch (Exception ex)
        {
            await repository.RollbackAsync(cancellationToken);

            if (DomainTransientFailureClassifier.TryClassify(ex, nameof(Handle), out var transient))
            {
                throw transient;
            }

            throw;
        }

        return mapper.Map<AccountContactRoleResponse>(entity);
    }
}

