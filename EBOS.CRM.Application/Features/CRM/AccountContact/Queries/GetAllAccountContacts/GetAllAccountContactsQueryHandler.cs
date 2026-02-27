using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;

public class GetAllAccountContactsQueryHandler(IAccountContactRepository repository, ITenantContext tenantContext, IMapper mapper)
    : IRequestHandler<GetAllAccountContactsQuery, PagedResult<AccountContactResponse>>
{
    public async Task<PagedResult<AccountContactResponse>> Handle(GetAllAccountContactsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var effectiveTenantId = tenantContext.TenantId > 0 ? tenantContext.TenantId : request.TenantId;
        if (entities.Any(x => x.TenantId != effectiveTenantId))
        {
            throw new InvalidOperationException("Account contact tenant mismatch.");
        }

        var items = mapper.Map<IReadOnlyCollection<AccountContactResponse>>(entities);
        var total = items.Count;
        return new PagedResult<AccountContactResponse>(items, total);
    }
}
