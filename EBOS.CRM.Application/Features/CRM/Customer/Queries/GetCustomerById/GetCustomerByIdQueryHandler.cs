using System;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(ICustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, CustomerResponse?>
{
    private readonly ICustomerRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<CustomerResponse?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<CustomerResponse>(entity);
    }
}




