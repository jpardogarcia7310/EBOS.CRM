using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Queries.GetAllQuotes;

public record GetAllQuotesQuery(int PageNumber = 1, int PageSize = 50) : IRequest<PagedResult<QuoteResponse>>;
