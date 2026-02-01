using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;

public class GetAllBranchOfficesQueryHandler(IBranchOfficeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBranchOfficesQuery, PagedResponse<BranchOfficeResponse>>
{
    private readonly IBranchOfficeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<BranchOfficeResponse>> Handle(GetAllBranchOfficesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<BranchOfficeResponse>>(result.Items);
        return new PagedResponse<BranchOfficeResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




