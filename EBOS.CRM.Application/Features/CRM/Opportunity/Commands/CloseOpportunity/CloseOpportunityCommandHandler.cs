using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;

public class CloseOpportunityCommandHandler(IOpportunityRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser) : IRequestHandler<CloseOpportunityCommand, OpportunityResponse?>
{
    public async Task<OpportunityResponse?> Handle(CloseOpportunityCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var oldValues = AuditSerialization.Serialize(entity);

        entity.StageId = request.OpportunityRequest.StageId;
        entity.CloseReason = request.OpportunityRequest.CloseReason;
        entity.Probability = request.OpportunityRequest.IsWon ? 1m : 0m;

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
                Entity: nameof(Domain.Entities.CRM.Opportunity),
                RegisterId: entity.Id,
                OldValues: oldValues,
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

        return new OpportunityResponse(
            entity.Id,
            entity.TenantId,
            entity.Name,
            entity.StageId,
            entity.OwnerUserId,
            entity.CustomerId,
            entity.ExpectedCloseDate,
            entity.Amount,
            entity.Probability,
            entity.Source,
            entity.SourceLeadId,
            entity.CloseReason,
            !entity.Erased);
    }
}
