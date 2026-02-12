using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseSla;

public class AssignCaseSlaCommandHandler(
    ICaseRepository repository,
    ISlaRepository slaRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper) : IRequestHandler<AssignCaseSlaCommand, CaseResponse?>
{
    public async Task<CaseResponse?> Handle(AssignCaseSlaCommand request, CancellationToken cancellationToken)
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
            throw new InvalidOperationException("Cannot change SLA for a closed case.");
        }

        var sla = await slaRepository.GetByIdAsync(entityRequest.SlaId, cancellationToken)
            ?? throw new InvalidOperationException("SLA not found.");
        if (sla.TenantId != entity.TenantId)
        {
            throw new InvalidOperationException("SLA tenant mismatch.");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.SlaId = entityRequest.SlaId;
        entity.UpdateDueAt(sla.CalculateDueAt(DateTime.UtcNow));

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
