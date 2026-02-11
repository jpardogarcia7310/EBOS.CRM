using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetAllSlas;

public class GetAllSlasQueryHandler(ISlaRepository repository, IMapper mapper)
    : IRequestHandler<GetAllSlasQuery, PagedResult<SlaResponse>>
{
    public async Task<PagedResult<SlaResponse>> Handle(GetAllSlasQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<SlaResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<SlaResponse>(items, total);
    }
}
