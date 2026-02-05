namespace EBOS.CRM.Application.Services.Interfaces;

public interface ICurrentUserContext
{
    long UserId { get; }
    long TenantId { get; }
    string CorrelationId { get; }
}
