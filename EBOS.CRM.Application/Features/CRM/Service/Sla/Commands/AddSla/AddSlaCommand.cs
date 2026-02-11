using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.AddSla;

public record AddSlaCommand(AddSlaRequest SlaRequest) : IRequest<SlaResponse>;
