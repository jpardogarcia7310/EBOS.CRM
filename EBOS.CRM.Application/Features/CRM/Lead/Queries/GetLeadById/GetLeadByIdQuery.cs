using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Queries.GetLeadById;

public record GetLeadByIdQuery(long Id) : IRequest<LeadResponse?>;
