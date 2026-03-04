using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivitiesByCaseId;

public class GetCaseActivitiesByCaseIdQueryHandler(ICaseActivityRepository repository, IMapper mapper)
    : IRequestHandler<GetCaseActivitiesByCaseIdQuery, PagedResult<CaseActivityResponse>>
{
    public async Task<PagedResult<CaseActivityResponse>> Handle(GetCaseActivitiesByCaseIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllByCaseIdPagedAsync(request.CaseId, request.PageNumber,
            request.PageSize, request.Status, request.From, request.To, cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<CaseActivityResponse>>(entities);
        var total = await repository.CountByCaseIdAsync(request.CaseId, request.Status, request.From, request.To,
            cancellationToken);
        return new PagedResult<CaseActivityResponse>(items, total);
    }
}
