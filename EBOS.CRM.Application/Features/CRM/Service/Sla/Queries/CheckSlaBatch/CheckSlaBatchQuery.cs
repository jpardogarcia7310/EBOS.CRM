using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;

public record CheckSlaBatchQuery(CheckSlaBatchRequest Request)
    : IRequest<PagedResult<SlaCheckResponse>>;
