using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Queries.GetCustomerPreferencesByCustomer;

public class GetCustomerPreferencesByCustomerQueryHandler(ICustomerPreferenceRepository repository, IMapper mapper)
    : IRequestHandler<GetCustomerPreferencesByCustomerQuery, PagedResult<CustomerPreferenceResponse>>
{
    public async Task<PagedResult<CustomerPreferenceResponse>> Handle(GetCustomerPreferencesByCustomerQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllAsync(cancellationToken);
        var filtered = entities.Where(x => x.CustomerId == request.CustomerId
                                           && x.TenantId == request.TenantId)
            .ToList();
        var total = filtered.Count;
        var itemsPage = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = mapper.Map<IReadOnlyCollection<CustomerPreferenceResponse>>(itemsPage);
        return new PagedResult<CustomerPreferenceResponse>(items, total);
    }
}
