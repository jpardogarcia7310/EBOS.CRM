using FluentValidation;

namespace EBOS.CRM.Application.Features.Countries.Commands.UpdateCountry;

public sealed class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El campo Name es obligatorio.").WithErrorCode("VAL_NAME_REQUIRED")
            .MaximumLength(200).WithMessage("El campo Name no puede superar 200 caracteres.").WithErrorCode("VAL_NAME_MAXLEN");
        RuleFor(x => x.Iso31661A2Code)
            .NotEmpty().WithMessage("El campo Iso31661A2Code es obligatorio.").WithErrorCode("VAL_ISOA2_REQUIRED")
            .Length(2).WithMessage("El campo Iso31661A2Code debe tener 2 caracteres.").WithErrorCode("VAL_ISOA2_LENGTH");
        RuleFor(x => x.Iso31661A3Code)
            .NotEmpty().WithMessage("El campo Iso31661A3Code es obligatorio.").WithErrorCode("VAL_ISOA3_REQUIRED")
            .Length(3).WithMessage("El campo Iso31661A3Code debe tener 3 caracteres.").WithErrorCode("VAL_ISOA3_LENGTH");
        RuleFor(x => x.Iso31661NumCode)
            .NotEmpty().WithMessage("El campo Iso31661NumCode es obligatorio.").WithErrorCode("VAL_ISONUM_REQUIRED")
            .MaximumLength(10).WithMessage("El campo Iso31661NumCode no puede superar 10 caracteres.").WithErrorCode("VAL_ISONUM_MAXLEN");
        RuleFor(x => x.Domain)
            .NotEmpty().WithMessage("El campo Domain es obligatorio.").WithErrorCode("VAL_DOMAIN_REQUIRED")
            .MaximumLength(50).WithMessage("El campo Domain no puede superar 50 caracteres.").WithErrorCode("VAL_DOMAIN_MAXLEN");
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("El campo Currency es obligatorio.").WithErrorCode("VAL_CURRENCY_REQUIRED")
            .MaximumLength(100).WithMessage("El campo Currency no puede superar 100 caracteres.").WithErrorCode("VAL_CURRENCY_MAXLEN");
        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("El campo CurrencyCode es obligatorio.").WithErrorCode("VAL_CURRENCYCODE_REQUIRED")
            .MaximumLength(10).WithMessage("El campo CurrencyCode no puede superar 10 caracteres.").WithErrorCode("VAL_CURRENCYCODE_MAXLEN");
        RuleFor(x => x.InternationalPhoneCode)
            .NotEmpty().WithMessage("El campo InternationalPhoneCode es obligatorio.").WithErrorCode("VAL_PHONE_REQUIRED")
            .MaximumLength(20).WithMessage("El campo InternationalPhoneCode no puede superar 20 caracteres.").WithErrorCode("VAL_PHONE_MAXLEN");
    }
}