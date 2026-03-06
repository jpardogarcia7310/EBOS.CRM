using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetTenantQuotaById;

public class GetTenantQuotaByIdQueryHandler(ITenantQuotaRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetTenantQuotaByIdQuery, TenantQuotaResponse?>
{
    public async Task<TenantQuotaResponse?> Handle(GetTenantQuotaByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = referenceLookupService is null
            ? await repository.GetByIdAsync(request.Id, cancellationToken)
            : await referenceLookupService.GetTenantQuotaByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<TenantQuotaResponse>(entity);
    }
}
