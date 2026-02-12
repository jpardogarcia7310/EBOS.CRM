using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.Customer.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler(ICustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCustomersQuery, PagedResult<CustomerResponse>>
{
    private readonly ICustomerRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<CustomerResponse>> Handle(GetAllCustomersQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize,
            cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CustomerResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<CustomerResponse>(items, total);
    }
}










