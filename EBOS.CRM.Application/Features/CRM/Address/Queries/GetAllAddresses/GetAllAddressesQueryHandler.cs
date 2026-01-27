using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Queries.GetAllAddresses;

public class GetAllAddressesQueryHandler(IAddressRepository repository, IMapper mapper) 
    : IRequestHandler<GetAllAddressQuery, IEnumerable<AddressResponse>>
{
    private readonly IAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IEnumerable<AddressResponse>> Handle(GetAllAddressQuery request, 
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();
        
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<AddressResponse>>(entities);
    }
}
