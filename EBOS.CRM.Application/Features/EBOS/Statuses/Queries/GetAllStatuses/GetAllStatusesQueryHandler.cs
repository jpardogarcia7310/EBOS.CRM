using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.Statuses.Queries.GetAllStatuses;

public class GetAllStatusesQueryHandler(IStatusRepository repository, IMapper mapper)
    : IRequestHandler<GetAllStatusesQuery, PagedResult<StatusResponse>>
{
    private readonly IStatusRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<StatusResponse>> Handle(GetAllStatusesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<StatusResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<StatusResponse>(items, total);
    }
}










