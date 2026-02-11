using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;

public class GetAllBranchOfficesQueryHandler(IBranchOfficeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBranchOfficesQuery, PagedResult<BranchOfficeResponse>>
{
    private readonly IBranchOfficeRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<BranchOfficeResponse>> Handle(GetAllBranchOfficesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber,
            request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<BranchOfficeResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<BranchOfficeResponse>(items, total);
    }
}










