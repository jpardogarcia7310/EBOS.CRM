using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;

public class GetAccountHierarchyByAccountQueryHandler(IAccountHierarchyRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountHierarchyByAccountQuery, PagedResult<AccountHierarchyResponse>>
{
    public async Task<PagedResult<AccountHierarchyResponse>> Handle(GetAccountHierarchyByAccountQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var itemsPage = await repository.GetByAccountPagedAsync(
            request.TenantId,
            request.CorporateCustomerId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        var total = await repository.CountByAccountAsync(
            request.TenantId,
            request.CorporateCustomerId,
            cancellationToken);

        var items = mapper.Map<IReadOnlyCollection<AccountHierarchyResponse>>(itemsPage);
        return new PagedResult<AccountHierarchyResponse>(items, total);
    }
}
