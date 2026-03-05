namespace EBOS.CRM.Domain.Exceptions;

public sealed class DomainRuleViolationException(string message, string code = "DOMAIN_RULE_VIOLATION", 
    Exception? innerException = null) : DomainException(code, message, retryable: false, innerException);
