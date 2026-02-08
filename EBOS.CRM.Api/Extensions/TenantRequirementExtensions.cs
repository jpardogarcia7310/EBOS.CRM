namespace EBOS.CRM.Api.Extensions;

public static class TenantRequirementExtensions
{
    public static IApplicationBuilder UseTenantRequirement(this IApplicationBuilder app)
        => app.UseMiddleware<TenantRequirementMiddleware>();
}
