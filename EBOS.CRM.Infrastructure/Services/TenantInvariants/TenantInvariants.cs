using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Infrastructure.Services.TenantInvariants;

public static class TenantInvariants
{
    public static void EnsureTenantAssigned(ITenantScopedEntity entity)
    {
        if (entity.TenantId <= 0)
        {
            throw new InvalidOperationException("TenantId is required.");
        }
    }
}
