using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetAllCustomerAddresses;

public class GetAllCustomerAddressesQueryHandler(ICustomerAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCustomerAddressesQuery, IReadOnlyCollection<CustomerAddressResponse>>
{
    private readonly ICustomerAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<CustomerAddressResponse>> Handle(GetAllCustomerAddressesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<CustomerAddressResponse>>(entities);
    }
}









