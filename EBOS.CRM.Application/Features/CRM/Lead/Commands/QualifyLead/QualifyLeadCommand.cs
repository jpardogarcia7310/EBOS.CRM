using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.QualifyLead;

public record QualifyLeadCommand(long Id, QualifyLeadRequest LeadRequest) : IRequest<LeadResponse?>;
