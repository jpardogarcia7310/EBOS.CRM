using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;

public class PatchCreditAccountCommandHandler(ICreditAccountRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser) : IRequestHandler<PatchCreditAccountCommand, CreditAccountResponse?>
{
    public async Task<CreditAccountResponse?> Handle(PatchCreditAccountCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var oldValues = AuditSerialization.Serialize(entity);

        if (request.CreditAccountRequest.MaxAmount.HasValue)
            entity.MaxAmount = request.CreditAccountRequest.MaxAmount.Value;
        if (request.CreditAccountRequest.UsedAmount.HasValue)
            entity.UsedAmount = request.CreditAccountRequest.UsedAmount.Value;
        if (request.CreditAccountRequest.CustomerId.HasValue)
            entity.CustomerId = request.CreditAccountRequest.CustomerId.Value;

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
                Entity: nameof(Domain.Entities.CRM.CreditAccount),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);

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

        return new CreditAccountResponse(
            entity.Id,
            entity.TenantId,
            entity.MaxAmount,
            entity.UsedAmount,
            entity.AvailableAmount,
            entity.CustomerId,
            !entity.Erased);
    }
}





