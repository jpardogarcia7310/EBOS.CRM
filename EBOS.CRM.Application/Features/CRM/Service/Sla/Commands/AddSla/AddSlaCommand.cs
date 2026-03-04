using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.AddSla;

public record AddSlaCommand(AddSlaRequest SlaRequest) : IRequest<SlaResponse>;
