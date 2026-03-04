using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;

public record CheckCaseSlaQuery(CheckCaseSlaRequest SlaRequest) : IRequest<SlaCheckResponse?>;
