using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetAllBranchOfficeAddresses;

public class GetAllBranchOfficeAddressesQueryHandler(IBranchOfficeAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBranchOfficeAddressesQuery, PagedResult<BranchOfficeAddressResponse>>
{
    private readonly IBranchOfficeAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<BranchOfficeAddressResponse>> Handle(GetAllBranchOfficeAddressesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<BranchOfficeAddressResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<BranchOfficeAddressResponse>(items, total);
    }
}










