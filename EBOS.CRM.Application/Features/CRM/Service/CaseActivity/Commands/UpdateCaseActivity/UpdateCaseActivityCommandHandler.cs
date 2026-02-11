using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.UpdateCaseActivity;

public class UpdateCaseActivityCommandHandler(
    ICaseActivityRepository repository,
    ICaseRepository caseRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper) : IRequestHandler<UpdateCaseActivityCommand, CaseActivityResponse?>
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
            ?? throw new InvalidOperationException("Case not found.");
        if (caseEntity.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Case tenant mismatch.");
        }
        if (entityRequest.CaseId != entity.CaseId)
        {
            throw new InvalidOperationException("CaseId cannot be changed for an activity.");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        mapper.Map(entityRequest, entity);

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
