namespace EBOS.CRM.Domain.Interfaces.Services.EBOS;

public interface ITenantOperationalValidationService
{
    void EnsureTenantIdIsPositive(long tenantId);
    void EnsureActorUserIdIsPositive(long actorUserId);
}
