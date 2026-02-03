using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetAllTaxInformationAddresses;

public class GetAllTaxInformationAddressesQueryHandler(ITaxInformationAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTaxInformationAddressesQuery, IReadOnlyCollection<TaxInformationAddressResponse>>
{
    private readonly ITaxInformationAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<TaxInformationAddressResponse>> Handle(GetAllTaxInformationAddressesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<TaxInformationAddressResponse>>(entities);
    }
}









