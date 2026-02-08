using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.AddressesType.Query.GetAllAddressesType;

public class GetAllAddressesTypeQueryHandler(IAddressTypeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllAddressesTypeQuery, PagedResult<AddressTypeResponse>>
{
    private readonly IAddressTypeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<AddressTypeResponse>> Handle(GetAllAddressesTypeQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<AddressTypeResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<AddressTypeResponse>(items, total);
    }
}


