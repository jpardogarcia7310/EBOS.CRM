using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
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
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<AssignCaseSlaCommand, CaseResponse?>
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
            throw new DomainRuleViolationException("Cannot change SLA for a closed case.", "DOMAIN_RULE_VIOLATION_CASE_CLOSED_SLA_CHANGE");
        }

        var sla = await slaRepository.GetByIdAsync(entityRequest.SlaId, cancellationToken)
            ?? throw new DomainValidationException("SLA not found.", "DOMAIN_VALIDATION_SLA_NOT_FOUND");
        if (sla.TenantId != entity.TenantId)
        {
            throw new DomainConflictException("SLA tenant mismatch.", "DOMAIN_CONFLICT_SLA_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.AssignSla(entityRequest.SlaId, sla.CalculateDueAt(DateTime.UtcNow));

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
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Case),
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

        return mapper.Map<CaseResponse>(entity);
    }
}
