using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.AddressesType.Query.GetAllAddressesType;

public class GetAllAddressesTypeQueryHandler(IAddressTypeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllAddressesTypeQuery, IReadOnlyCollection<AddressTypeResponse>>
{
    private readonly IAddressTypeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<AddressTypeResponse>> Handle(GetAllAddressesTypeQuery request,
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<AddressTypeResponse>>(entities);
    }
}










