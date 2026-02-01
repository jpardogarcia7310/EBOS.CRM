using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetAllTaxInformationAddresses;

public class GetAllTaxInformationAddressesQueryHandler(ITaxInformationAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTaxInformationAddressesQuery, PagedResponse<TaxInformationAddressResponse>>
{
    private readonly ITaxInformationAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<TaxInformationAddressResponse>> Handle(GetAllTaxInformationAddressesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<TaxInformationAddressResponse>>(result.Items);
        return new PagedResponse<TaxInformationAddressResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




