using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;

public class DeleteAccountContactRoleCommandHandler(
    IAccountContactRoleRepository repository,
    IAuditService auditService,
    ICurrentUserContext currentUser)
    : IRequestHandler<DeleteAccountContactRoleCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountContactRoleCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRoleRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRoleRequest));

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return false;

        if (entity.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Account contact role tenant mismatch.");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.DeleteAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Delete,
                Entity: nameof(Domain.Entities.CRM.AccountContactRole),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: null,
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            await repository.CommitAsync(cancellationToken);
        }
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return true;
    }
}
