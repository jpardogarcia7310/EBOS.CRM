using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;

public class GetAccountContactByIdQueryHandler(IAccountContactRepository repository, ITenantContext tenantContext, IMapper mapper)
    : IRequestHandler<GetAccountContactByIdQuery, AccountContactResponse?>
{
    public async Task<AccountContactResponse?> Handle(GetAccountContactByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is not null && tenantContext.TenantId > 0 && entity.TenantId != tenantContext.TenantId)
        {
            throw new InvalidOperationException("Account contact tenant mismatch.");
        }

        return entity is null ? null : mapper.Map<AccountContactResponse>(entity);
    }
}
