using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.UpdateQueue;

public class UpdateQueueCommandHandler(
    IQueueRepository repository,
    ICaseRepository caseRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<UpdateQueueCommand, QueueResponse?>
{
    public async Task<QueueResponse?> Handle(UpdateQueueCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.QueueRequest ?? throw new ArgumentNullException(nameof(request.QueueRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var oldValues = AuditSerialization.Serialize(entity);
        var previousIsActive = entity.IsActive;
        var previousDefaultOwnerUserId = entity.DefaultOwnerUserId;
        mapper.Map(entityRequest, entity);
        entity.IsActive = previousIsActive;
        entity.DefaultOwnerUserId = previousDefaultOwnerUserId;
        var openCount = entityRequest.IsActive
            ? 0
            : await caseRepository.CountOpenByQueueIdAsync(entity.Id, cancellationToken);
        entity.ToggleActive(entityRequest.IsActive, openCount > 0);
        entity.AssignDefaultOwner(entityRequest.DefaultOwnerUserId);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Queue),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Queue),
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

        return mapper.Map<QueueResponse>(entity);
    }
}
