namespace EBOS.CRM.Domain.Exceptions;

public sealed class TransientDomainFailureException(string message, string code = "TRANSIENT_DOMAIN_FAILURE", 
    Exception? innerException = null) : DomainException(code, message, retryable: true, innerException);
