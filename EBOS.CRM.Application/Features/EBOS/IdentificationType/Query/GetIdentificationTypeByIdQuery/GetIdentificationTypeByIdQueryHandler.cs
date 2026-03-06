using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery;

public class GetIdentificationTypeByIdQueryHandler(IIdentificationTypeRepository repository, IMapper mapper, IEbosReferenceLookupService? referenceLookupService = null)
    : IRequestHandler<GetIdentificationTypeByIdQuery, IdentificationTypeResponse?>
{
    private readonly IIdentificationTypeRepository _repository = repository ??
                                                                 throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IdentificationTypeResponse?> Handle(GetIdentificationTypeByIdQuery request,
        CancellationToken cancellationToken)
    {
        // 👇 Throws OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();

        var entity = referenceLookupService is null
            ? await _repository.GetByIdAsync(request.Id, cancellationToken)
            : await referenceLookupService.GetIdentificationTypeByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<IdentificationTypeResponse>(entity);
    }
}



