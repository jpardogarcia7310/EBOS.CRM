using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Queries.GetCustomerConsentsByCustomer;

public class GetCustomerConsentsByCustomerQueryHandler(ICustomerConsentRepository repository, IMapper mapper)
    : IRequestHandler<GetCustomerConsentsByCustomerQuery, PagedResult<CustomerConsentResponse>>
{
    public async Task<PagedResult<CustomerConsentResponse>> Handle(GetCustomerConsentsByCustomerQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllAsync(cancellationToken);
        var filtered = entities.Where(x => x.CustomerId == request.CustomerId).ToList();
        var total = filtered.Count;
        var itemsPage = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = mapper.Map<IReadOnlyCollection<CustomerConsentResponse>>(itemsPage);
        return new PagedResult<CustomerConsentResponse>(items, total);
    }
}
