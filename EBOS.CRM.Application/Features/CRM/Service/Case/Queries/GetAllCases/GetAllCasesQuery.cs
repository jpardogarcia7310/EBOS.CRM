using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetAllCases;

public record GetAllCasesQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<CaseResponse>>;
