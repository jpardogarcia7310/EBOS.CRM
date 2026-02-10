using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Queries.GetAllLeads;

public class GetAllLeadsQueryHandler(ILeadRepository repository, IMapper mapper)
    : IRequestHandler<GetAllLeadsQuery, PagedResult<LeadResponse>>
{
    public async Task<PagedResult<LeadResponse>> Handle(GetAllLeadsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize, 
            cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<LeadResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<LeadResponse>(items, total);
    }
}
