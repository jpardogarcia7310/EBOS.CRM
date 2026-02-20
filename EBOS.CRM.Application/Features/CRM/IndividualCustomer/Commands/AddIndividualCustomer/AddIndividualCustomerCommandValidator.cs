using EBOS.CRM.Application.Validation;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.AddIndividualCustomer;

public class AddIndividualCustomerCommandValidator : AbstractValidator<AddIndividualCustomerCommand>
{
    private readonly IIdentificationTypeRepository _identificationTypeRepository;
    private readonly IValidationCatalogService _validationCatalog;

    public AddIndividualCustomerCommandValidator(IIdentificationTypeRepository identificationTypeRepository,
        IValidationCatalogService validationCatalog)
    {
        _identificationTypeRepository = identificationTypeRepository;
        _validationCatalog = validationCatalog;

        RuleFor(x => x.IndividualCustomerRequest).NotNull();
        RuleFor(x => x.IndividualCustomerRequest.Code).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.Email).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.Phone).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.FirstName).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.LastName).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.IdentificationNumber).MaximumLength(500);
        RuleFor(x => x.IndividualCustomerRequest.StatusId).GreaterThan(0);
        RuleFor(x => x.IndividualCustomerRequest.IdentificationTypeId).GreaterThan(0);

        RuleFor(x => x.IndividualCustomerRequest.IdentificationTypeId)
            .MustAsync(IdentificationTypeExistsAsync)
            .WithMessage("IdentificationTypeId does not exist.");

        RuleFor(x => x.IndividualCustomerRequest)
            .MustAsync(IdentificationNumberMatchesTypeAsync)
            .WithMessage("IdentificationNumber does not match the configured mask.");

        RuleFor(x => x.IndividualCustomerRequest)
            .MustAsync(PhoneMatchesDefaultAsync)
            .WithMessage("Phone does not match the configured mask.");
    }

    private async Task<bool> IdentificationTypeExistsAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _identificationTypeRepository.GetByIdAsync(id, cancellationToken);
        return entity is not null;
    }

    private async Task<bool> IdentificationNumberMatchesTypeAsync(global::EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer.AddIndividualCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
        {
            return true;
        }

        var type = await _identificationTypeRepository.GetByIdAsync(request.IdentificationTypeId, cancellationToken);
        if (type is null)
        {
            return true;
        }

        var pattern = await _validationCatalog.GetPatternAsync(ValidationRuleKeys.Identification(type.Code),
            cancellationToken);
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.IdentificationNumber, pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
    }

    private async Task<bool> PhoneMatchesDefaultAsync(global::EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer.AddIndividualCustomerRequest request,
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




