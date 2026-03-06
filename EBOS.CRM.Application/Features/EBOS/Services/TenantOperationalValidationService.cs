using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.Services;

public sealed class TenantOperationalValidationService : ITenantOperationalValidationService
{
    public void EnsureTenantIdIsPositive(long tenantId)
    {
        if (tenantId <= 0)
        {
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }
    }

    public void EnsureActorUserIdIsPositive(long actorUserId)
    {
        if (actorUserId <= 0)
        {
            throw new DomainValidationException("Actor user id must be a positive value.", "DOMAIN_VALIDATION_ACTOR_USER_ID_POSITIVE");
        }
    }
}
