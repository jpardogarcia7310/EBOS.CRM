using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;

public class GetAllStatusesQueryHandler(IStatusRepository repository, IMapper mapper) : IRequestHandler<GetAllStatusesQuery, IReadOnlyCollection<StatusResponse>>
{
    private readonly IStatusRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<StatusResponse>> Handle(GetAllStatusesQuery request,
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token has already been canceled.
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<StatusResponse>>(entities);
    }
}









