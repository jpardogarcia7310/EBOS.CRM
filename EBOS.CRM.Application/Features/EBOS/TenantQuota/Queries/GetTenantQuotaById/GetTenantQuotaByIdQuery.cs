using EBOS.CRM.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetTenantQuotaById;

public record GetTenantQuotaByIdQuery(long Id) : IRequest<TenantQuotaResponse?>;
