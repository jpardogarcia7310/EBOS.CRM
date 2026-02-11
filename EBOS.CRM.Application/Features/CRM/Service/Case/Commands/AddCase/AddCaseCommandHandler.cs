using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AddCase;

public class AddCaseCommandHandler(
    ICaseRepository repository,
    IQueueRepository queueRepository,
    ISlaRepository slaRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper) : IRequestHandler<AddCaseCommand, CaseResponse>
{
    public async Task<CaseResponse> Handle(AddCaseCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CaseRequest ?? throw new ArgumentNullException(nameof(request.CaseRequest));
        var queue = await queueRepository.GetByIdAsync(entityRequest.QueueId, cancellationToken)
            ?? throw new InvalidOperationException("Queue not found.");
        if (!queue.IsActive)
        {
            throw new InvalidOperationException("Queue is not active.");
        }
        if (queue.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Queue tenant mismatch.");
        }

        var sla = await slaRepository.GetByIdAsync(entityRequest.SlaId, cancellationToken)
            ?? throw new InvalidOperationException("SLA not found.");
        if (sla.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("SLA tenant mismatch.");
        }

        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.Case>(entityRequest);
        entity.SetStatus(entityRequest.Status);
        entity.SetPriority(entityRequest.Priority);

        var dueAt = entityRequest.DueAt ?? sla.CalculateDueAt(DateTime.UtcNow);
        entity.UpdateDueAt(dueAt);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.Case),
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

        return mapper.Map<CaseResponse>(entity);
    }
}
