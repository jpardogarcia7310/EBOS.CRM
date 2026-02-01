using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;

public class GetAllStatusesQueryHandler(IStatusRepository repository, IMapper mapper) : IRequestHandler<GetAllStatusesQuery, PagedResponse<StatusResponse>>
{
    public async Task<PagedResponse<StatusResponse>> Handle(GetAllStatusesQuery request, 
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token has already been canceled.
        cancellationToken.ThrowIfCancellationRequested();

        var result = await repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<StatusResponse>>(result.Items);
        return new PagedResponse<StatusResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




