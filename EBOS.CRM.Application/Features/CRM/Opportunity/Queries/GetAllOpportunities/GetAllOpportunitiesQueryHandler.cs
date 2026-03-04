using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetAllOpportunities;

public class GetAllOpportunitiesQueryHandler(IOpportunityRepository repository, IMapper mapper)
    : IRequestHandler<GetAllOpportunitiesQuery, PagedResult<OpportunityResponse>>
{
    public async Task<PagedResult<OpportunityResponse>> Handle(GetAllOpportunitiesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize,
            cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<OpportunityResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<OpportunityResponse>(items, total);
    }
}
