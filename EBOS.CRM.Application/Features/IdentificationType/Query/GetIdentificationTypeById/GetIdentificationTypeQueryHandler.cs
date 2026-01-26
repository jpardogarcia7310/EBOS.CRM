using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeById;

public class GetIdentificationTypeQueryHandler(IIdentificationTypeRepository repository, IMapper mapper) 
    : IRequestHandler<GetIdentificationTypeQuery, IdentificationTypeResponse?>
{
    private readonly IIdentificationTypeRepository _repository = repository ?? 
                                                                 throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IdentificationTypeResponse?> Handle(GetIdentificationTypeQuery request, CancellationToken cancellationToken)
    {
        // 👇 Throws OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();
        
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<IdentificationTypeResponse>(entity);
    }
}