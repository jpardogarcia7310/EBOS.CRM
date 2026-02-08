using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetAllTenantConfigurations;

public record GetAllTenantConfigurationsQuery(int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<TenantConfigurationResponse>>;
