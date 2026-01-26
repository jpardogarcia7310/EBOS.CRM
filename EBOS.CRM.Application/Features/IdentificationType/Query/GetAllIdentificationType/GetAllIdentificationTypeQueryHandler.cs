using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.AddressesType.Query.GetAllAddressesType;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType;

public class GetAllIdentificationTypeQueryHandler(IIdentificationTypeRepository repository, IMapper mapper) 
    : IRequestHandler<GetAllIdentificationTypeQuery, IEnumerable<IdentificationTypeResponse>>
{
    private readonly IIdentificationTypeRepository _repository = repository ?? 
                                                                 throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IEnumerable<IdentificationTypeResponse>> Handle(GetAllIdentificationTypeQuery request, 
        CancellationToken cancellationToken)
    {
        // 👇 This throws an OperationCancelledException if the token is already canceled
        cancellationToken.ThrowIfCancellationRequested();
        
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<IdentificationTypeResponse>>(entities);
    }
}
