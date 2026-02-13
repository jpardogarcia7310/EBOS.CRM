using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;

public class GetAllAccountContactsQueryHandler(IAccountContactRepository repository, IMapper mapper)
    : IRequestHandler<GetAllAccountContactsQuery, PagedResult<AccountContactResponse>>
{
    public async Task<PagedResult<AccountContactResponse>> Handle(GetAllAccountContactsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<AccountContactResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<AccountContactResponse>(items, total);
    }
}
