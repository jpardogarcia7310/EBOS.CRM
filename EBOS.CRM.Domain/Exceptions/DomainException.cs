namespace EBOS.CRM.Domain.Exceptions;

public abstract class DomainException(
    DomainErrorTaxonomyType taxonomyType,
    string code,
    string message,
    bool retryable = false,
    Exception? innerException = null) : Exception(message, innerException)
{
    public DomainErrorTaxonomyType TaxonomyType { get; } = taxonomyType;

    public string Code { get; } = code;

    public bool Retryable { get; } = retryable;
}
