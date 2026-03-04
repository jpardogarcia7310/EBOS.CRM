using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.QualifyLead;

public record QualifyLeadCommand(long Id, QualifyLeadRequest LeadRequest) : IRequest<LeadResponse?>;
