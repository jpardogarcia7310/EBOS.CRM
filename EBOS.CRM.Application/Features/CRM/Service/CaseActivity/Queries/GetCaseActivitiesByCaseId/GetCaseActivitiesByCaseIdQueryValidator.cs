using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivitiesByCaseId;

public class GetCaseActivitiesByCaseIdQueryValidator : AbstractValidator<GetCaseActivitiesByCaseIdQuery>
{
    public GetCaseActivitiesByCaseIdQueryValidator()
    {
        RuleFor(x => x.CaseId).GreaterThan(0);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);

        When(x => !string.IsNullOrWhiteSpace(x.Status), () =>
        {
            var allowed = new[]
            {
                global::EBOS.CRM.Domain.Entities.CRM.CaseActivity.StatusOpen,
                global::EBOS.CRM.Domain.Entities.CRM.CaseActivity.StatusInProgress,
                global::EBOS.CRM.Domain.Entities.CRM.CaseActivity.StatusCompleted,
                global::EBOS.CRM.Domain.Entities.CRM.CaseActivity.StatusCancelled
            };

            RuleFor(x => x.Status)
                .Must(s => allowed.Contains(s!, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of {string.Join(", ", allowed)}.")
                .WithState(_ => new { allowedValues = allowed });
        });

        When(x => x.From.HasValue && x.To.HasValue, () =>
        {
            RuleFor(x => x.To)
                .GreaterThanOrEqualTo(x => x.From!.Value)
                .WithMessage("To must be greater than or equal to From.");
        });
    }
}
