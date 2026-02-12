using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Queries.GetAllOpportunityStages;

public class GetAllOpportunityStagesQueryHandler(IOpportunityStageRepository repository, IMapper mapper)
    : IRequestHandler<GetAllOpportunityStagesQuery, PagedResult<OpportunityStageResponse>>
{
    public async Task<PagedResult<OpportunityStageResponse>> Handle(GetAllOpportunityStagesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber,
            request.PageSize, cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<OpportunityStageResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<OpportunityStageResponse>(items, total);
    }
}
