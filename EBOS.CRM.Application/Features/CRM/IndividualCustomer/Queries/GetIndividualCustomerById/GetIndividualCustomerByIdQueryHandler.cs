using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetIndividualCustomerById;

public class GetIndividualCustomerByIdQueryHandler(IIndividualCustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetIndividualCustomerByIdQuery, IndividualCustomerResponse?>
{
    private readonly IIndividualCustomerRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IndividualCustomerResponse?> Handle(GetIndividualCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<IndividualCustomerResponse>(entity);
    }
}




