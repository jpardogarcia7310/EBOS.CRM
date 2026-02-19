using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;

public class FindCustomerDuplicatesQueryValidator : AbstractValidator<FindCustomerDuplicatesQuery>
{
    public FindCustomerDuplicatesQueryValidator(ICustomerDedupeNormalizationService normalizationService)
    {
        RuleFor(x => x.Request).NotNull();
        When(x => x.Request != null, () =>
        {
            RuleFor(x => x.Request.TenantId).GreaterThan(0);
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Request.Email)
                           || !string.IsNullOrWhiteSpace(x.Request.Phone)
                           || !string.IsNullOrWhiteSpace(x.Request.TaxId)
                           || !string.IsNullOrWhiteSpace(x.Request.IdentificationNumber))
                .WithMessage("At least one matching field is required.");
        });

        When(x => x.Request != null && !string.IsNullOrWhiteSpace(x.Request.Email), () =>
        {
            RuleFor(x => x.Request.Email!)
                .NotEmpty()
                .MaximumLength(CustomerDedupeValidationRules.MaxEmailLength)
                .EmailAddress();
        });

        When(x => x.Request != null && !string.IsNullOrWhiteSpace(x.Request.Phone), () =>
        {
            RuleFor(x => x.Request.Phone!)
                .Must(phone =>
                {
                    var trimmed = phone.Trim();
                    var normalized = normalizationService.NormalizePhone(trimmed);
                    return !string.IsNullOrWhiteSpace(normalized)
                           && normalized.Length == trimmed.Length
                           && normalized.Length <= CustomerDedupeValidationRules.MaxPhoneDigits
                           && System.Text.RegularExpressions.Regex.IsMatch(trimmed,
                               CustomerDedupeValidationRules.DigitsPattern);
                })
                .WithMessage("Phone must be normalized and contain only digits.");
        });

        When(x => x.Request != null && !string.IsNullOrWhiteSpace(x.Request.TaxId), () =>
        {
            RuleFor(x => x.Request.TaxId!)
                .NotEmpty()
                .MaximumLength(CustomerDedupeValidationRules.MaxTaxIdLength)
                .Matches(CustomerDedupeValidationRules.AlphanumericPattern)
                .WithMessage("TaxId must be alphanumeric.");
        });

        When(x => x.Request != null && !string.IsNullOrWhiteSpace(x.Request.IdentificationNumber), () =>
        {
            RuleFor(x => x.Request.IdentificationNumber!)
                .NotEmpty()
                .MaximumLength(CustomerDedupeValidationRules.MaxIdentificationNumberLength)
                .Matches(CustomerDedupeValidationRules.AlphanumericPattern)
                .WithMessage("IdentificationNumber must be alphanumeric.");
        });

        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}
