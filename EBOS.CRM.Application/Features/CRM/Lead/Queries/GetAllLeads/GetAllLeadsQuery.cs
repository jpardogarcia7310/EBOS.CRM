using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Queries.GetAllLeads;

public record GetAllLeadsQuery(int PageNumber = 1, int PageSize = 50) : IRequest<PagedResult<LeadResponse>>;
