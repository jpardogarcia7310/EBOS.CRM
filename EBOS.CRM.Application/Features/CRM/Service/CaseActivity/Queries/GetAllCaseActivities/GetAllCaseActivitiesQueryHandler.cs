using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetAllCaseActivities;

public class GetAllCaseActivitiesQueryHandler(ICaseActivityRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCaseActivitiesQuery, PagedResult<CaseActivityResponse>>
{
    public async Task<PagedResult<CaseActivityResponse>> Handle(GetAllCaseActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<CaseActivityResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<CaseActivityResponse>(items, total);
    }
}
