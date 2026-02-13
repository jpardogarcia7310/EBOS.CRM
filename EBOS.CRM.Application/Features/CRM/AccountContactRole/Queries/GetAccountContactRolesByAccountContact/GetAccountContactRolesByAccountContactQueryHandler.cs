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

        var itemsPage = await repository.GetByAccountContactPagedAsync(
            request.TenantId,
            request.AccountContactId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        var total = await repository.CountByAccountContactAsync(
            request.TenantId,
            request.AccountContactId,
            cancellationToken);

        var items = mapper.Map<IReadOnlyCollection<AccountContactRoleResponse>>(itemsPage);
        return new PagedResult<AccountContactRoleResponse>(items, total);
    }
}
