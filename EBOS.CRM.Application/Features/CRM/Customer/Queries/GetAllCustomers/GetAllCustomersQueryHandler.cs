using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.Customer.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler(ICustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCustomersQuery, IReadOnlyCollection<CustomerResponse>>
{
    private readonly ICustomerRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<CustomerResponse>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<CustomerResponse>>(entities);
    }
}









