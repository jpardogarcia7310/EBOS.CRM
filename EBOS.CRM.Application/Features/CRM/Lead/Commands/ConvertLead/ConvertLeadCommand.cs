using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.ConvertLead;

public record ConvertLeadCommand(long Id, ConvertLeadRequest LeadRequest) : IRequest<OpportunityResponse?>;
