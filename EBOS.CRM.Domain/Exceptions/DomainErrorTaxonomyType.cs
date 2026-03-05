namespace EBOS.CRM.Domain.Exceptions;

public enum DomainErrorTaxonomyType
{
    DomainValidation = 1,
    DomainConflict = 2,
    DomainRuleViolation = 3,
    TransientDomainFailure = 4
}
