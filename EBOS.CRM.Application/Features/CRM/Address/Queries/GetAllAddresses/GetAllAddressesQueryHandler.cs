using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.Address.Queries.GetAllAddresses;

public class GetAllAddressesQueryHandler(IAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllAddressQuery, PagedResponse<AddressResponse>>
{
    private readonly IAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<AddressResponse>> Handle(GetAllAddressQuery request,
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<AddressResponse>>(result.Items);
        return new PagedResponse<AddressResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




