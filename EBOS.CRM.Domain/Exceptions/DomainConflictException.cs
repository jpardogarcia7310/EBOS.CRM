namespace EBOS.CRM.Domain.Exceptions;

public sealed class DomainConflictException(
    string message,
    string code = "DOMAIN_CONFLICT",
    bool retryable = false,
    Exception? innerException = null) : DomainException(
    DomainErrorTaxonomyType.DomainConflict,
    code,
    message,
    retryable,
    innerException);
