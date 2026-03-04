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

        var itemsPage = await repository.GetByCustomerPagedAsync(
            request.TenantId,
            request.CustomerId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        var total = await repository.CountByCustomerAsync(
            request.TenantId,
            request.CustomerId,
            cancellationToken);

        var items = mapper.Map<IReadOnlyCollection<CustomerPreferenceResponse>>(itemsPage);
        return new PagedResult<CustomerPreferenceResponse>(items, total);
    }
}
