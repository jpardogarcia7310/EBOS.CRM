namespace EBOS.CRM.Domain.Interfaces.Services;

public interface ICurrentUserContext : ITenantContext
{
    long UserId { get; }
    string CorrelationId { get; }
}