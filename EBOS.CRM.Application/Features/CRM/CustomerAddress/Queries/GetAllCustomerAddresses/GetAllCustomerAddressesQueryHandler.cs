using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetAllCustomerAddresses;

public class GetAllCustomerAddressesQueryHandler(ICustomerAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCustomerAddressesQuery, PagedResult<CustomerAddressResponse>>
{
    private readonly ICustomerAddressRepository _repository = repository ??
                                                              throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<CustomerAddressResponse>> Handle(GetAllCustomerAddressesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber,
            request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CustomerAddressResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<CustomerAddressResponse>(items, total);
    }
}










