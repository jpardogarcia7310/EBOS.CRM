using MapsterMapper;
using MediatR;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetValidationRuleById;

public class GetValidationRuleByIdQueryHandler(IValidationRuleRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetValidationRuleByIdQuery, ValidationRuleResponse>
{
    private readonly IValidationRuleRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<ValidationRuleResponse> Handle(GetValidationRuleByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = referenceLookupService is null
            ? await _repository.GetByIdAsync(request.Id, cancellationToken)
            : await referenceLookupService.GetValidationRuleByIdOrThrowAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException("ValidationRule not found.");
        }

        return _mapper.Map<ValidationRuleResponse>(entity);
    }
}
