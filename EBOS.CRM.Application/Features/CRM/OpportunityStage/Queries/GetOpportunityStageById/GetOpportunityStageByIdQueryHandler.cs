using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Queries.GetOpportunityStageById;

public class GetOpportunityStageByIdQueryHandler(IOpportunityStageRepository repository, IMapper mapper)
    : IRequestHandler<GetOpportunityStageByIdQuery, OpportunityStageResponse?>
{
    public async Task<OpportunityStageResponse?> Handle(GetOpportunityStageByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity == null ? null : mapper.Map<OpportunityStageResponse>(entity);
    }
}
