using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRolesByAccountContact;

public class GetAccountContactRolesByAccountContactQueryHandler(IAccountContactRoleRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountContactRolesByAccountContactQuery, PagedResult<AccountContactRoleResponse>>
{
    public async Task<PagedResult<AccountContactRoleResponse>> Handle(GetAccountContactRolesByAccountContactQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllAsync(cancellationToken);
        var filtered = entities.Where(x => x.AccountContactId == request.AccountContactId
                                           && x.TenantId == request.TenantId)
            .ToList();
        var total = filtered.Count;
        var itemsPage = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = mapper.Map<IReadOnlyCollection<AccountContactRoleResponse>>(itemsPage);
        return new PagedResult<AccountContactRoleResponse>(items, total);
    }
}
