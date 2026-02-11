using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;

public record CheckCaseSlaQuery(CheckCaseSlaRequest SlaRequest) : IRequest<SlaCheckResponse?>;
