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

        var entities = await repository.GetAllAsync(cancellationToken);
        var filtered = entities.Where(x => x.TenantId == request.TenantId
                                           && (x.ParentCorporateCustomerId == request.CorporateCustomerId
                                               || x.ChildCorporateCustomerId == request.CorporateCustomerId))
            .ToList();
        var total = filtered.Count;
        var itemsPage = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = mapper.Map<IReadOnlyCollection<AccountHierarchyResponse>>(itemsPage);
        return new PagedResult<AccountHierarchyResponse>(items, total);
    }
}
