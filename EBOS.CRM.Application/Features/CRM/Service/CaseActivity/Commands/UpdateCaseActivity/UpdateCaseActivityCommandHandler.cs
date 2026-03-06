using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.UpdateCaseActivity;

public class UpdateCaseActivityCommandHandler(
    ICaseActivityRepository repository,
    ICaseRepository caseRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<UpdateCaseActivityCommand, CaseActivityResponse?>
{
    public async Task<CaseActivityResponse?> Handle(UpdateCaseActivityCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.ActivityRequest ?? throw new ArgumentNullException(nameof(request.ActivityRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var caseEntity = await caseRepository.GetByIdAsync(entity.CaseId, cancellationToken)
            ?? throw new DomainValidationException("Case not found.", "DOMAIN_VALIDATION_CASE_NOT_FOUND");
        if (caseEntity.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Case tenant mismatch.", "DOMAIN_CONFLICT_CASE_TENANT_MISMATCH");
        }
        if (entityRequest.CaseId != entity.CaseId)
        {
            throw new DomainRuleViolationException("CaseId cannot be changed for an activity.", "DOMAIN_RULE_VIOLATION_CASE_ACTIVITY_CASE_IMMUTABLE");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        var currentStatus = entity.Status;
        mapper.Map(entityRequest, entity);
        entity.Status = currentStatus;
        entity.SetStatus(entityRequest.Status);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.CaseActivity),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.CaseActivity),
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

        return mapper.Map<CaseActivityResponse>(entity);
    }
}
