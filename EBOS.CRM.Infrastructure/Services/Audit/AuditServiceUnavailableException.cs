namespace EBOS.CRM.Infrastructure.Services.Audit;

public sealed class AuditServiceUnavailableException(string message, Exception? inner = null)
    : Exception(message, inner);
