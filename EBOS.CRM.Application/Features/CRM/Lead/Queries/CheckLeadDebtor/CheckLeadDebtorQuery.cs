using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Queries.CheckLeadDebtor;

public sealed record CheckLeadDebtorQuery(LeadDebtorCheckRequest Request)
    : IRequest<LeadDebtorCheckResponse>;
