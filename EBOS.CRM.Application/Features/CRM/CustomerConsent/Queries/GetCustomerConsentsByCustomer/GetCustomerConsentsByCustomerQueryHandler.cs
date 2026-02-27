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

        var itemsPage = await repository.GetLatestByCustomerPagedAsync(
            request.TenantId,
            request.CustomerId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        var total = await repository.CountLatestByCustomerAsync(
            request.TenantId,
            request.CustomerId,
            cancellationToken);

        var items = mapper.Map<IReadOnlyCollection<CustomerConsentResponse>>(itemsPage);
        return new PagedResult<CustomerConsentResponse>(items, total);
    }
}
