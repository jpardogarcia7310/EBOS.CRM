using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.UpdateLead;

public record UpdateLeadCommand(long Id, UpdateLeadRequest LeadRequest) : IRequest<LeadResponse?>;
