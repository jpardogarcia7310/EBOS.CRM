using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.DisqualifyLead;

public record DisqualifyLeadCommand(long Id, DisqualifyLeadRequest LeadRequest) : IRequest<LeadResponse?>;
