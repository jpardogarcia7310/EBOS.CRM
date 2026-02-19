using EBOS.CRM.Application.Options;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;

public class UpdateIndividualCustomerCommandValidator : AbstractValidator<UpdateIndividualCustomerCommand>
{
    private readonly IIdentificationTypeRepository _identificationTypeRepository;
    private readonly ValidationCatalogOptions _options;

    public UpdateIndividualCustomerCommandValidator(IIdentificationTypeRepository identificationTypeRepository,
        IOptions<ValidationCatalogOptions> options)
    {
        _identificationTypeRepository = identificationTypeRepository;
        _options = options.Value ?? new ValidationCatalogOptions();

        RuleFor(x => x.Id).GreaterThan(0);
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
    }

    private async Task<bool> IdentificationTypeExistsAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await _identificationTypeRepository.GetByIdAsync(id, cancellationToken);
        return entity is not null;
    }

    private async Task<bool> IdentificationNumberMatchesTypeAsync(global::EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer.UpdateIndividualCustomerRequest request,
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

        if (!_options.IdentificationPatternsByTypeCode.TryGetValue(type.Code, out var pattern) || string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return Regex.IsMatch(request.IdentificationNumber, pattern, RegexOptions.CultureInvariant);
    }
}




