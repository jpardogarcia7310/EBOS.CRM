using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseQueue;

public class AssignCaseQueueCommandHandler(
    ICaseRepository repository,
    IQueueRepository queueRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper) : IRequestHandler<AssignCaseQueueCommand, CaseResponse?>
{
    public async Task<CaseResponse?> Handle(AssignCaseQueueCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CaseRequest ?? throw new ArgumentNullException(nameof(request.CaseRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (entity.ClosedAt.HasValue)
        {
            throw new InvalidOperationException("Cannot change queue for a closed case.");
        }

        var queue = await queueRepository.GetByIdAsync(entityRequest.QueueId, cancellationToken)
            ?? throw new InvalidOperationException("Queue not found.");
        if (!queue.IsActive)
        {
            throw new InvalidOperationException("Queue is not active.");
        }
        if (queue.TenantId != entity.TenantId)
        {
            throw new InvalidOperationException("Queue tenant mismatch.");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.AssignQueue(entityRequest.QueueId);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Case),
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

        return mapper.Map<CaseResponse>(entity);
    }
}
