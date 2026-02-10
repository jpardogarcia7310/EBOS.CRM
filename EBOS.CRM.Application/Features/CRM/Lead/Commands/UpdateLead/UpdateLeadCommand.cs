using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.UpdateLead;

public record UpdateLeadCommand(long Id, UpdateLeadRequest LeadRequest) : IRequest<LeadResponse?>;
