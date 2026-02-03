using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetBranchOfficeAddressById;

public class GetBranchOfficeAddressByIdQueryHandler(IBranchOfficeAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetBranchOfficeAddressByIdQuery, BranchOfficeAddressResponse?>
{
    private readonly IBranchOfficeAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<BranchOfficeAddressResponse?> Handle(GetBranchOfficeAddressByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<BranchOfficeAddressResponse>(entity);
    }
}




