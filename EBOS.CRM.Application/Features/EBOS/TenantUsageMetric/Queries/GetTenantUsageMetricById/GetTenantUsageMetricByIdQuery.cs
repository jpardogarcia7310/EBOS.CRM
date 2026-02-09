using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;

public record GetTenantUsageMetricByIdQuery(long Id) : IRequest<TenantUsageMetricResponse?>;
