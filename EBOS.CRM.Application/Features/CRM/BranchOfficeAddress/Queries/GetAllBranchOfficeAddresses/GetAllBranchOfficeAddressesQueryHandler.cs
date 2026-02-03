using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetAllBranchOfficeAddresses;

public class GetAllBranchOfficeAddressesQueryHandler(IBranchOfficeAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBranchOfficeAddressesQuery, IReadOnlyCollection<BranchOfficeAddressResponse>>
{
    private readonly IBranchOfficeAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<BranchOfficeAddressResponse>> Handle(GetAllBranchOfficeAddressesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<BranchOfficeAddressResponse>>(entities);
    }
}









