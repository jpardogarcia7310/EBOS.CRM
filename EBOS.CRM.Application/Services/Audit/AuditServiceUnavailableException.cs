namespace EBOS.CRM.Application.Services.Audit;

public sealed class AuditServiceUnavailableException(string message, Exception? inner = null)
    : Exception(message, inner);
