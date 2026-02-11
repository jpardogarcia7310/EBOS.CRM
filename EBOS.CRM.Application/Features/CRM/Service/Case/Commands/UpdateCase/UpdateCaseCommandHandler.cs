using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.UpdateCase;

public class UpdateCaseCommandHandler(
    ICaseRepository repository,
    IQueueRepository queueRepository,
    ISlaRepository slaRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper) : IRequestHandler<UpdateCaseCommand, CaseResponse?>
{
    public async Task<CaseResponse?> Handle(UpdateCaseCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CaseRequest ?? throw new ArgumentNullException(nameof(request.CaseRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        _ = await queueRepository.GetByIdAsync(entityRequest.QueueId, cancellationToken)
            ?? throw new InvalidOperationException("Queue not found.");
        _ = await slaRepository.GetByIdAsync(entityRequest.SlaId, cancellationToken)
            ?? throw new InvalidOperationException("SLA not found.");

        var oldValues = AuditSerialization.Serialize(entity);
        var currentStatus = entity.Status;

        mapper.Map(entityRequest, entity);

        if (!string.Equals(currentStatus, entityRequest.Status, StringComparison.OrdinalIgnoreCase))
        {
            entity.Status = currentStatus;
            entity.SetStatus(entityRequest.Status);
        }

        entity.SetPriority(entityRequest.Priority);
        entity.UpdateDueAt(entityRequest.DueAt);

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
