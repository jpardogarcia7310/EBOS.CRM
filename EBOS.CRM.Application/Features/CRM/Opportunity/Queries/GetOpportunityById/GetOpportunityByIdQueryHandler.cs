using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetOpportunityById;

public class GetOpportunityByIdQueryHandler(IOpportunityRepository repository, IMapper mapper)
    : IRequestHandler<GetOpportunityByIdQuery, OpportunityResponse?>
{
    public async Task<OpportunityResponse?> Handle(GetOpportunityByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<OpportunityResponse>(entity);
    }
}
