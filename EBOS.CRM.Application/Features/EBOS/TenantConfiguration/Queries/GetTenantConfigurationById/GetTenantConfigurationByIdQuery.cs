using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetTenantConfigurationById;

public record GetTenantConfigurationByIdQuery(long Id) : IRequest<TenantConfigurationResponse?>;
