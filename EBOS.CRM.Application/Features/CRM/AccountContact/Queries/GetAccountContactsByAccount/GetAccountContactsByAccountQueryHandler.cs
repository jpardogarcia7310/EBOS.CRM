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

        var entities = await repository.GetAllAsync(cancellationToken);
        var filtered = entities.Where(x => x.CorporateCustomerId == request.CorporateCustomerId
                                           && x.TenantId == request.TenantId)
            .ToList();
        var total = filtered.Count;
        var itemsPage = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = mapper.Map<IReadOnlyCollection<AccountContactResponse>>(itemsPage);
        return new PagedResult<AccountContactResponse>(items, total);
    }
}
