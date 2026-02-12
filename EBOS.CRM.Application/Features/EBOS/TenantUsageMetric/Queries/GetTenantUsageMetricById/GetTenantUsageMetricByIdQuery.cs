using EBOS.CRM.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;

public record GetTenantUsageMetricByIdQuery(long Id) : IRequest<TenantUsageMetricResponse?>;
