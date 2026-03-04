using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByStatus;

public sealed class GetCustomerPrivacyRequestsByStatusQueryHandler(ICustomerPrivacyRequestRepository repository)
    : IRequestHandler<GetCustomerPrivacyRequestsByStatusQuery, PagedResult<CustomerPrivacyRequestResponse>>
{
    public async Task<PagedResult<CustomerPrivacyRequestResponse>> Handle(GetCustomerPrivacyRequestsByStatusQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var itemsPage = await repository.GetByStatusPagedAsync(
            request.TenantId, request.Status, request.PageNumber, request.PageSize, cancellationToken);
        var total = await repository.CountByStatusAsync(request.TenantId, request.Status, cancellationToken);
        var items = itemsPage.Select(x => x.ToResponse()).ToList();
        return new PagedResult<CustomerPrivacyRequestResponse>(items, total);
    }
}
