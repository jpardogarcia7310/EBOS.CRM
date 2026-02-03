using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;

public class GetAllCorporateCustomersQueryHandler(ICorporateCustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCorporateCustomersQuery, IReadOnlyCollection<CorporateCustomerResponse>>
{
    private readonly ICorporateCustomerRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<CorporateCustomerResponse>> Handle(GetAllCorporateCustomersQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<CorporateCustomerResponse>>(entities);
    }
}









