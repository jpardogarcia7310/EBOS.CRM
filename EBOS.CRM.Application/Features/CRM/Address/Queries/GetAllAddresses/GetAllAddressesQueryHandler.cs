using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.Address.Queries.GetAllAddresses;

public class GetAllAddressesQueryHandler(IAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllAddressesQuery, PagedResult<AddressResponse>>
{
    private readonly IAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<AddressResponse>> Handle(GetAllAddressesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<AddressResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<AddressResponse>(items, total);
    }
}










