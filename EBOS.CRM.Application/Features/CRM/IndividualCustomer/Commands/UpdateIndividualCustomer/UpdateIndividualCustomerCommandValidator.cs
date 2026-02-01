using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;

public class UpdateIndividualCustomerCommandValidator : AbstractValidator<UpdateIndividualCustomerCommand>
{
    public UpdateIndividualCustomerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.IndividualCustomerRequest).NotNull();
        RuleFor(x => x.IndividualCustomerRequest.Code).NotEmpty(); RuleFor(x => x.IndividualCustomerRequest.Email).NotEmpty(); RuleFor(x => x.IndividualCustomerRequest.Phone).NotEmpty(); RuleFor(x => x.IndividualCustomerRequest.FirstName).NotEmpty(); RuleFor(x => x.IndividualCustomerRequest.LastName).NotEmpty();
        RuleFor(x => x.IndividualCustomerRequest.IdentificationNumber).MaximumLength(500);
        RuleFor(x => x.IndividualCustomerRequest.StatusId).GreaterThan(0); RuleFor(x => x.IndividualCustomerRequest.IdentificationTypeId).GreaterThan(0);
    }
}
