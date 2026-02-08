using EBOS.CRM.Domain.Interfaces;

namespace EBOS.CRM.Domain.Services;

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
