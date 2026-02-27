using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactsByAccount;

public class GetAccountContactsByAccountQueryHandler(IAccountContactRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountContactsByAccountQuery, PagedResult<AccountContactResponse>>
{
    public async Task<PagedResult<AccountContactResponse>> Handle(GetAccountContactsByAccountQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var itemsPage = await repository.GetByCorporateCustomerPagedAsync(
            request.TenantId,
            request.CorporateCustomerId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        var total = await repository.CountByCorporateCustomerAsync(
            request.TenantId,
            request.CorporateCustomerId,
            cancellationToken);

        var items = mapper.Map<IReadOnlyCollection<AccountContactResponse>>(itemsPage);
        return new PagedResult<AccountContactResponse>(items, total);
    }
}
