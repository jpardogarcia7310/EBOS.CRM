using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.AddressesType.Query.GetAddressTypeById;

public class GetAddressTypeByIdQueryHandler(IAddressTypeRepository repository, IMapper mapper)
    : IRequestHandler<GetAddressTypeByIdQuery, AddressTypeResponse?>
{
    private readonly IAddressTypeRepository _repository = repository ??
                                                          throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<AddressTypeResponse?> Handle(GetAddressTypeByIdQuery request, CancellationToken cancellationToken)
    {
        // 👇 Throws OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<AddressTypeResponse>(entity);
    }
}



