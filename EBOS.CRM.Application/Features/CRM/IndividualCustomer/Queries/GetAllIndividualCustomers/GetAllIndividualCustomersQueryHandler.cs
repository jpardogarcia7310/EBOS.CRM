using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetAllIndividualCustomers;

public class GetAllIndividualCustomersQueryHandler(IIndividualCustomerRepository repository, IMapper mapper)
    : IRequestHandler<GetAllIndividualCustomersQuery, PagedResult<IndividualCustomerResponse>>
{
    private readonly IIndividualCustomerRepository _repository = repository ??
                                                                 throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<IndividualCustomerResponse>> Handle(GetAllIndividualCustomersQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber,
            request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<IndividualCustomerResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<IndividualCustomerResponse>(items, total);
    }
}










