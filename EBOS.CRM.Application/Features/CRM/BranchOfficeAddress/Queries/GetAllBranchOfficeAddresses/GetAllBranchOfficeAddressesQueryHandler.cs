using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetAllBranchOfficeAddresses;

public class GetAllBranchOfficeAddressesQueryHandler(IBranchOfficeAddressRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBranchOfficeAddressesQuery, PagedResponse<BranchOfficeAddressResponse>>
{
    private readonly IBranchOfficeAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<BranchOfficeAddressResponse>> Handle(GetAllBranchOfficeAddressesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<BranchOfficeAddressResponse>>(result.Items);
        return new PagedResponse<BranchOfficeAddressResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




