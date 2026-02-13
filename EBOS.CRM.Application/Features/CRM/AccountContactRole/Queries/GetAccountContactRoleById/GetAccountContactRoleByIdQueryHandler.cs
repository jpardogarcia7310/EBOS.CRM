using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;

public class GetAccountContactRoleByIdQueryHandler(IAccountContactRoleRepository repository, IMapper mapper)
    : IRequestHandler<GetAccountContactRoleByIdQuery, AccountContactRoleResponse?>
{
    public async Task<AccountContactRoleResponse?> Handle(GetAccountContactRoleByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<AccountContactRoleResponse>(entity);
    }
}
