using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetAllTenantUsageMetrics;

public class GetAllTenantUsageMetricsQueryHandler(ITenantUsageMetricRepository repository, IMapper mapper)
    : IRequestHandler<GetAllTenantUsageMetricsQuery, PagedResult<TenantUsageMetricResponse>>
{
    private readonly ITenantUsageMetricRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<TenantUsageMetricResponse>> Handle(GetAllTenantUsageMetricsQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<TenantUsageMetricResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<TenantUsageMetricResponse>(items, total);
    }
}
