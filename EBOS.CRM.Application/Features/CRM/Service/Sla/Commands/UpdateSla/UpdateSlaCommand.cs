using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.UpdateSla;

public record UpdateSlaCommand(long Id, UpdateSlaRequest SlaRequest) : IRequest<SlaResponse?>;
