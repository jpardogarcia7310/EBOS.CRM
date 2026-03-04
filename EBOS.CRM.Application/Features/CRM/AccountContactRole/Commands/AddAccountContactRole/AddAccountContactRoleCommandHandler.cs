using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
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
    IMapper mapper)
    : IRequestHandler<AddAccountContactRoleCommand, AccountContactRoleResponse>
{
    public async Task<AccountContactRoleResponse> Handle(AddAccountContactRoleCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRoleRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRoleRequest));

        var accountContact = await accountContactRepository.GetByIdAsync(entityRequest.AccountContactId, cancellationToken)
            ?? throw new InvalidOperationException("Account contact not found.");
        if (accountContact.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Account contact tenant mismatch.");
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
