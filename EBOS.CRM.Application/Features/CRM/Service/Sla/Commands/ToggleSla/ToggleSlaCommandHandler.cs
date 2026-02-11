using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.ToggleSla;

public class ToggleSlaCommandHandler(
    ISlaRepository repository,
    ICaseRepository caseRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper) : IRequestHandler<ToggleSlaCommand, SlaResponse?>
{
    public async Task<SlaResponse?> Handle(ToggleSlaCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.SlaRequest ?? throw new ArgumentNullException(nameof(request.SlaRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var oldValues = AuditSerialization.Serialize(entity);
        if (!entityRequest.IsActive)
        {
            var openCount = await caseRepository.CountOpenBySlaIdAsync(entity.Id, cancellationToken);
            if (openCount > 0)
            {
                throw new InvalidOperationException("SLA has open cases and cannot be deactivated.");
            }
        }

        entity.IsActive = entityRequest.IsActive;

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.Sla),
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

        return mapper.Map<SlaResponse>(entity);
    }
}
