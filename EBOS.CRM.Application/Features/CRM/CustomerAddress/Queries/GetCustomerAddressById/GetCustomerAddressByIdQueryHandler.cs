using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetCustomerAddressById;

public class GetCustomerAddressByIdQueryHandler(ICustomerAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetCustomerAddressByIdQuery, CustomerAddressResponse?>
{
    private readonly ICustomerAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<CustomerAddressResponse?> Handle(GetCustomerAddressByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<CustomerAddressResponse>(entity);
    }
}




