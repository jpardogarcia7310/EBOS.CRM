using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByCustomer;

public sealed class GetCustomerPrivacyRequestsByCustomerQueryHandler(ICustomerPrivacyRequestRepository repository)
    : IRequestHandler<GetCustomerPrivacyRequestsByCustomerQuery, PagedResult<CustomerPrivacyRequestResponse>>
{
    public async Task<PagedResult<CustomerPrivacyRequestResponse>> Handle(GetCustomerPrivacyRequestsByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var itemsPage = await repository.GetByCustomerPagedAsync(
            request.TenantId, request.CustomerId, request.PageNumber, request.PageSize, cancellationToken);
        var total = await repository.CountByCustomerAsync(request.TenantId, request.CustomerId, cancellationToken);
        var items = itemsPage.Select(x => x.ToResponse()).ToList();
        return new PagedResult<CustomerPrivacyRequestResponse>(items, total);
    }
}
