using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.AddCaseActivity;

public class AddCaseActivityCommandHandler(
    ICaseActivityRepository repository,
    ICaseRepository caseRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper,
    IDomainOperationalEventPublisher? domainOperationalEventPublisher = null) : IRequestHandler<AddCaseActivityCommand, CaseActivityResponse>
{
    public async Task<CaseActivityResponse> Handle(AddCaseActivityCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.ActivityRequest ?? throw new ArgumentNullException(nameof(request.ActivityRequest));
        var caseEntity = await caseRepository.GetByIdAsync(entityRequest.CaseId, cancellationToken)
            ?? throw new DomainValidationException("Case not found.", "DOMAIN_VALIDATION_CASE_NOT_FOUND");
        if (caseEntity.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Case tenant mismatch.", "DOMAIN_CONFLICT_CASE_TENANT_MISMATCH");
        }
        if (caseEntity.ClosedAt.HasValue)
        {
            throw new DomainRuleViolationException("Cannot add activities to a closed case.", "DOMAIN_RULE_VIOLATION_CASE_CLOSED_ACTIVITY_ADD");
        }

        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.CaseActivity>(entityRequest);
        var initialStatus = entity.Status;
        entity.Status = string.Empty;
        entity.SetStatus(initialStatus);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.CaseActivity),
                RegisterId: entity.Id,
                OldValues: null,
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
