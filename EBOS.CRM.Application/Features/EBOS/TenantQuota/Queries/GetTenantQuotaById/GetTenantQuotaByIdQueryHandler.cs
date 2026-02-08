using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetTenantQuotaById;

public class GetTenantQuotaByIdQueryHandler(ITenantQuotaRepository repository, IMapper mapper)
    : IRequestHandler<GetTenantQuotaByIdQuery, TenantQuotaResponse?>
{
    public async Task<TenantQuotaResponse?> Handle(GetTenantQuotaByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<TenantQuotaResponse>(entity);
    }
}
