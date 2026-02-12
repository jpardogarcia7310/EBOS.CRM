using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetCorporateCustomerById;

public class GetCorporateCustomerByIdQueryHandler(ICorporateCustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetCorporateCustomerByIdQuery, CorporateCustomerResponse?>
{
    private readonly ICorporateCustomerRepository _repository = repository ??
                                                                throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<CorporateCustomerResponse?> Handle(GetCorporateCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<CorporateCustomerResponse>(entity);
    }
}




