using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;

public class AddAccountContactRoleCommandHandler(
    IAccountContactRoleRepository repository,
    IAccountContactRepository accountContactRepository,
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

        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.AccountContactRole>(entityRequest);
        entity.Activate(entityRequest.ValidFrom);
        if (entityRequest.ValidTo.HasValue)
        {
            entity.Deactivate(entityRequest.ValidTo.Value);
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
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
