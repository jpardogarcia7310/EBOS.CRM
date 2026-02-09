namespace EBOS.CRM.Application.Services.Interfaces;

public interface ICurrentUserContext : ITenantContext
{
    long UserId { get; }
    string CorrelationId { get; }
}
