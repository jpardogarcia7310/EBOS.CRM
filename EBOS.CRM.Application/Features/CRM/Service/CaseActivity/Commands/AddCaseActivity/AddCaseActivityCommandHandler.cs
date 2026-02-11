using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.AddCaseActivity;

public class AddCaseActivityCommandHandler(
    ICaseActivityRepository repository,
    ICaseRepository caseRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper) : IRequestHandler<AddCaseActivityCommand, CaseActivityResponse>
{
    public async Task<CaseActivityResponse> Handle(AddCaseActivityCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.ActivityRequest ?? throw new ArgumentNullException(nameof(request.ActivityRequest));
        var caseEntity = await caseRepository.GetByIdAsync(entityRequest.CaseId, cancellationToken)
            ?? throw new InvalidOperationException("Case not found.");
        if (caseEntity.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Case tenant mismatch.");
        }

        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.CaseActivity>(entityRequest);

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
