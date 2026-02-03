using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetAllTaxInformationAddresses;

public class GetAllTaxInformationAddressesQueryHandler(ITaxInformationAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTaxInformationAddressesQuery, PagedResult<TaxInformationAddressResponse>>
{
    private readonly ITaxInformationAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<TaxInformationAddressResponse>> Handle(GetAllTaxInformationAddressesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<TaxInformationAddressResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<TaxInformationAddressResponse>(items, total);
    }
}










