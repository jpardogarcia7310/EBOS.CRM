using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;

public class GetTenantUsageMetricByIdQueryHandler(ITenantUsageMetricRepository repository, IMapper mapper)
    : IRequestHandler<GetTenantUsageMetricByIdQuery, TenantUsageMetricResponse?>
{
    public async Task<TenantUsageMetricResponse?> Handle(GetTenantUsageMetricByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<TenantUsageMetricResponse>(entity);
    }
}
