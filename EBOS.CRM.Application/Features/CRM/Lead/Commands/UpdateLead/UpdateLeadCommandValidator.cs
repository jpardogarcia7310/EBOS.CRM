using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.UpdateLead;

public class UpdateLeadCommandValidator : AbstractValidator<UpdateLeadCommand>
{
    public UpdateLeadCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.LeadRequest).NotNull();

        When(x => x.LeadRequest != null, () =>
        {
            RuleFor(x => x.LeadRequest.Id).GreaterThan(0);
            RuleFor(x => x.LeadRequest.Source)
                .NotEmpty().MaximumLength(100);
            RuleFor(x => x.LeadRequest.Status)
                .NotEmpty().MaximumLength(50);
            RuleFor(x => x.LeadRequest.OwnerUserId)
                .GreaterThan(0);
            RuleFor(x => x.LeadRequest.CompanyName)
                .NotEmpty().MaximumLength(200);
            RuleFor(x => x.LeadRequest.ContactName)
                .NotEmpty().MaximumLength(150);
            RuleFor(x => x.LeadRequest.Email)
                .NotEmpty().MaximumLength(100);
            RuleFor(x => x.LeadRequest.Phone)
                .NotEmpty().MaximumLength(20);
            RuleFor(x => x.LeadRequest.EstimatedValue)
                .GreaterThanOrEqualTo(0).When(x => x.LeadRequest.EstimatedValue.HasValue);
            RuleFor(x => x.LeadRequest.Notes)
                .MaximumLength(2000);
        });
    }
}
