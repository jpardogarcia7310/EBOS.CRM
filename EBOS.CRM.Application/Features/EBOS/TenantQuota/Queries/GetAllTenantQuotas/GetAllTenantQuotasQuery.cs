using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetAllTenantQuotas;

public record GetAllTenantQuotasQuery(int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<TenantQuotaResponse>>;
