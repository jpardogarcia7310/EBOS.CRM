namespace EBOS.CRM.Domain.Interfaces;

public interface ITenantScopedEntity
{
    long TenantId { get; set; }
}
