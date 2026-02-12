using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetCaseById;

public record GetCaseByIdQuery(long Id) : IRequest<CaseResponse?>;
