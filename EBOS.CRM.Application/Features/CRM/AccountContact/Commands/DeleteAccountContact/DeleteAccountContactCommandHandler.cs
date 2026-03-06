using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;

public class DeleteAccountContactCommandHandler(
    IAccountContactRepository repository,
    IAuditService auditService,
    ICurrentUserContext currentUser)
    : IRequestHandler<DeleteAccountContactCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountContactCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRequest));

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return false;

        if (entity.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Account contact tenant mismatch.", "DOMAIN_CONFLICT_ACCOUNT_CONTACT_TENANT_MISMATCH");
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
                Entity: nameof(Domain.Entities.CRM.AccountContact),
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
