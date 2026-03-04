namespace EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

public interface ITenantScopedEntity
{
    long TenantId { get; set; }
}
