namespace EBOS.CRM.Domain.Exceptions;

public abstract class DomainException(string code, string message, bool retryable = false, 
    Exception? innerException = null) : Exception(message, innerException)
{
    public string Code { get; } = code;

    public bool Retryable { get; } = retryable;
}
