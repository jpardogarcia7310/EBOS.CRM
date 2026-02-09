using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.AddOpportunityStage;

public class AddOpportunityStageCommandHandler(IOpportunityStageRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper) 
    : IRequestHandler<AddOpportunityStageCommand, OpportunityStageResponse>
{
    public async Task<OpportunityStageResponse> Handle(AddOpportunityStageCommand request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.StageRequest ?? throw new ArgumentNullException(nameof(request.StageRequest));
        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage>(entityRequest);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.OpportunityStage),
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

        return mapper.Map<OpportunityStageResponse>(entity);
    }
}
