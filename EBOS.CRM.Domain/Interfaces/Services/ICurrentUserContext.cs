using EBOS.CRM.Domain.Interfaces.Services.EBOS;

namespace EBOS.CRM.Domain.Interfaces.Services;

public interface ICurrentUserContext : ITenantContext
{
    long UserId { get; }
    string CorrelationId { get; }
}