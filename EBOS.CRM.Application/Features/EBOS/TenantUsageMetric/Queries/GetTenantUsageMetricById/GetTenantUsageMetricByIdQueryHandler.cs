using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;

public class GetTenantUsageMetricByIdQueryHandler(ITenantUsageMetricRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetTenantUsageMetricByIdQuery, TenantUsageMetricResponse?>
{
    public async Task<TenantUsageMetricResponse?> Handle(GetTenantUsageMetricByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = referenceLookupService is null
            ? await repository.GetByIdAsync(request.Id, cancellationToken)
            : await referenceLookupService.GetTenantUsageMetricByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<TenantUsageMetricResponse>(entity);
    }
}
