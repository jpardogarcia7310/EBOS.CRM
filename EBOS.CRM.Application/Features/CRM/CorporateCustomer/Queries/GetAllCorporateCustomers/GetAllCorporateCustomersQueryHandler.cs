using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;

public class GetAllCorporateCustomersQueryHandler(ICorporateCustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCorporateCustomersQuery, PagedResponse<CorporateCustomerResponse>>
{
    private readonly ICorporateCustomerRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<CorporateCustomerResponse>> Handle(GetAllCorporateCustomersQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CorporateCustomerResponse>>(result.Items);
        return new PagedResponse<CorporateCustomerResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




