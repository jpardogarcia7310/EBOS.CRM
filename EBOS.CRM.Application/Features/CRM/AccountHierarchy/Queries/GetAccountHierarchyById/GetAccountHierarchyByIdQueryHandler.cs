using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyById;

public class GetAccountHierarchyByIdQueryHandler(IAccountHierarchyRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountHierarchyByIdQuery, AccountHierarchyResponse?>
{
    public async Task<AccountHierarchyResponse?> Handle(GetAccountHierarchyByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<AccountHierarchyResponse>(entity);
    }
}
