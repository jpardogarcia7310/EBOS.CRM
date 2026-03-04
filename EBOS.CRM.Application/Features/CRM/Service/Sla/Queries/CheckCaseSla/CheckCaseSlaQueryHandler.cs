using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;

public class CheckCaseSlaQueryHandler(
    ICaseRepository caseRepository,
    ISlaRepository slaRepository)
    : IRequestHandler<CheckCaseSlaQuery, SlaCheckResponse?>
{
    public async Task<SlaCheckResponse?> Handle(CheckCaseSlaQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.SlaRequest ?? throw new ArgumentNullException(nameof(request.SlaRequest));
        var caseEntity = await caseRepository.GetByIdAsync(entityRequest.CaseId, cancellationToken);
        if (caseEntity is null)
        {
            return null;
        }

        var sla = await slaRepository.GetByIdAsync(caseEntity.SlaId, cancellationToken);
        if (sla is null)
        {
            return null;
        }

        var isActive = sla.IsActiveAt(entityRequest.Now);
        var dueAt = caseEntity.DueAt ?? sla.CalculateDueAt(caseEntity.CreatedAt);
        var isBreached = isActive && sla.IsBreached(entityRequest.Now, dueAt);

        return new SlaCheckResponse(
            CaseId: caseEntity.Id,
            SlaId: sla.Id,
            DueAt: dueAt,
            IsBreached: isBreached,
            IsActive: isActive);
    }
}
