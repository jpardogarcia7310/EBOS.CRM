using EBOS.CRM.Application.Features.CRM.CustomerMerge;
using EBOS.CRM.Application.Validation;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.AddCustomer;

public class AddCustomerCommandValidator : AbstractValidator<AddCustomerCommand>
{
    private readonly IValidationCatalogService _validationCatalog;

    public AddCustomerCommandValidator(IValidationCatalogService validationCatalog)
    {
        _validationCatalog = validationCatalog;

        RuleFor(x => x.CustomerRequest).NotNull();
        When(x => x.CustomerRequest != null, () =>
        {
            RuleFor(x => x.CustomerRequest.Code).NotEmpty();
            RuleFor(x => x.CustomerRequest.Email).NotEmpty();
            RuleFor(x => x.CustomerRequest.Phone).NotEmpty();
            RuleFor(x => x.CustomerRequest.StatusId).GreaterThan(0);

            RuleFor(x => x.CustomerRequest.Email)
                .MaximumLength(CustomerDedupeValidationRules.MaxEmailLength)
                .EmailAddress();

            RuleFor(x => x.CustomerRequest.Phone)
                .MaximumLength(CustomerDedupeValidationRules.MaxPhoneDigits)
                .Matches(CustomerDedupeValidationRules.DigitsPattern);

            RuleFor(x => x.CustomerRequest)
                .MustAsync(PhoneMatchesDefaultAsync)
                .WithMessage("Phone does not match the configured mask.");
        });
    }

    private async Task<bool> PhoneMatchesDefaultAsync(global::EBOS.CRM.Contracts.Requests.CRM.Customer.AddCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return true;
        }

        var pattern = await _validationCatalog.GetPatternAsync(ValidationRuleKeys.Phone(ValidationRuleKeys.DefaultCountryKey),
            cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.Phone, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }
}




