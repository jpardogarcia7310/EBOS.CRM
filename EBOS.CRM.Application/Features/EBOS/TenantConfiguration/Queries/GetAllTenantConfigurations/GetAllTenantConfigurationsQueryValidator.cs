using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetAllTenantConfigurations;

public class GetAllTenantConfigurationsQueryValidator : AbstractValidator<GetAllTenantConfigurationsQuery>
{
    public GetAllTenantConfigurationsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
