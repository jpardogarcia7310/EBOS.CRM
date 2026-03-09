using MapsterMapper;
using MediatR;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetAllIdentificationType;

public class GetAllIdentificationTypeQueryHandler(IIdentificationTypeRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetAllIdentificationTypeQuery, PagedResult<IdentificationTypeResponse>>
{
    private readonly IIdentificationTypeRepository _repository = repository ??
                                                                 throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<IdentificationTypeResponse>> Handle(GetAllIdentificationTypeQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = referenceLookupService is null
            ? await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken)
            : await referenceLookupService.GetIdentificationTypesPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities);
        var total = referenceLookupService is null
            ? await _repository.CountAsync(cancellationToken)
            : await referenceLookupService.CountIdentificationTypesAsync(cancellationToken);
        return new PagedResult<IdentificationTypeResponse>(items, total);
    }
}










