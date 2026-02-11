using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivityById;

public record GetCaseActivityByIdQuery(long Id) : IRequest<CaseActivityResponse?>;
