using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivitiesByCaseId;

public record GetCaseActivitiesByCaseIdQuery(long CaseId, int PageNumber = 1, int PageSize = 10,
    string? Status = null, DateTime? From = null, DateTime? To = null)
    : IRequest<PagedResult<CaseActivityResponse>>;
