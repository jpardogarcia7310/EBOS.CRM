using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetAllCases;

public class GetAllCasesQueryHandler(ICaseRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCasesQuery, PagedResult<CaseResponse>>
{
    public async Task<PagedResult<CaseResponse>> Handle(GetAllCasesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<CaseResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<CaseResponse>(items, total);
    }
}
