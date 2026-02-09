using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.ApiTests.Domain;

public class TenantScopedEntityCoverageTest
{
    [Fact]
    public void All_Crm_Entities_Implement_ITenantScopedEntity()
    {
        var assembly = typeof(Address).Assembly;
        var entityTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => string.Equals(t.Namespace, "EBOS.CRM.Domain.Entities.CRM", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(entityTypes);

        var nonTenantScoped = entityTypes
            .Where(t => !typeof(ITenantScopedEntity).IsAssignableFrom(t))
            .Select(t => t.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        Assert.Empty(nonTenantScoped);
    }
}
