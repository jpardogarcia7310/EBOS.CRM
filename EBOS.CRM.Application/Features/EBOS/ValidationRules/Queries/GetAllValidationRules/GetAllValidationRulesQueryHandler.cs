using MapsterMapper;
using MediatR;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetAllValidationRules;

public class GetAllValidationRulesQueryHandler(IValidationRuleRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetAllValidationRulesQuery, PagedResult<ValidationRuleResponse>>
{
    private readonly IValidationRuleRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<ValidationRuleResponse>> Handle(GetAllValidationRulesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = referenceLookupService is null
            ? await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken)
            : await referenceLookupService.GetValidationRulesPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<ValidationRuleResponse>>(entities);
        var total = referenceLookupService is null
            ? await _repository.CountAsync(cancellationToken)
            : await referenceLookupService.CountValidationRulesAsync(cancellationToken);
        return new PagedResult<ValidationRuleResponse>(items, total);
    }
}
