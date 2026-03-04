using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetAllTenantUsageMetrics;

public record GetAllTenantUsageMetricsQuery(int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<TenantUsageMetricResponse>>;
