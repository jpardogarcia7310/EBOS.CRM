using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetAllCaseActivities;

public record GetAllCaseActivitiesQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<CaseActivityResponse>>;
