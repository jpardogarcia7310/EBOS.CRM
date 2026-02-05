using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;

public class GetAllCorporateCustomersQueryHandler(ICorporateCustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCorporateCustomersQuery, PagedResult<CorporateCustomerResponse>>
{
    private readonly ICorporateCustomerRepository _repository = repository ?? 
                                                                throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<CorporateCustomerResponse>> Handle(GetAllCorporateCustomersQuery request, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber,
            request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CorporateCustomerResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<CorporateCustomerResponse>(items, total);
    }
}










