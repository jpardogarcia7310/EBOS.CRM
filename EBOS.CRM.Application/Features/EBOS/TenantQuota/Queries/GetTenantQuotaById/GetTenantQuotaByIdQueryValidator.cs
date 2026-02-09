using FluentValidation;

namespace EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetTenantQuotaById;

public class GetTenantQuotaByIdQueryValidator : AbstractValidator<GetTenantQuotaByIdQuery>
{
    public GetTenantQuotaByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithErrorCode("VAL_ID_POSITIVE")
            .WithMessage("The identifier must be a positive integer greater than 0.");
    }
}
