using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;

public class GetAccountContactRoleByIdQueryHandler(IAccountContactRoleRepository repository, ITenantContext tenantContext, IMapper mapper)
    : IRequestHandler<GetAccountContactRoleByIdQuery, AccountContactRoleResponse?>
{
    public async Task<AccountContactRoleResponse?> Handle(GetAccountContactRoleByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is not null && tenantContext.TenantId > 0 && entity.TenantId != tenantContext.TenantId)
        {
            throw new InvalidOperationException("Account contact role tenant mismatch.");
        }

        return entity is null ? null : mapper.Map<AccountContactRoleResponse>(entity);
    }
}
