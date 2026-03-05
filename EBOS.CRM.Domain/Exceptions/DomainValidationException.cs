namespace EBOS.CRM.Domain.Exceptions;

public sealed class DomainValidationException(
    string message,
    string code = "DOMAIN_VALIDATION",
    Exception? innerException = null) : DomainException(
    DomainErrorTaxonomyType.DomainValidation,
    code,
    message,
    retryable: false,
    innerException);
