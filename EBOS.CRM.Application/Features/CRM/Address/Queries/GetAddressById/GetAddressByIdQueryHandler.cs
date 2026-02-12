using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;

public class GetAddressByIdQueryHandler(IAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAddressByIdQuery, AddressResponse?>
{
    private readonly IAddressRepository _repository = repository ??
                                                          throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<AddressResponse?> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        // 👇 Throws OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<AddressResponse>(entity);
    }
}



