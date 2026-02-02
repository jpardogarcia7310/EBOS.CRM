namespace EBOS.CRM.Application.Services.Interfaces;

public interface ICurrentUserContext
{
    long UserId { get; }
    string CorrelationId { get; }
}
