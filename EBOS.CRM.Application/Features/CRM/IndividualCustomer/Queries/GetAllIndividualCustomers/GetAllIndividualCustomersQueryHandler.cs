using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetAllIndividualCustomers;

public class GetAllIndividualCustomersQueryHandler(IIndividualCustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetAllIndividualCustomersQuery, IReadOnlyCollection<IndividualCustomerResponse>>
{
    private readonly IIndividualCustomerRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<IndividualCustomerResponse>> Handle(GetAllIndividualCustomersQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<IndividualCustomerResponse>>(entities);
    }
}









