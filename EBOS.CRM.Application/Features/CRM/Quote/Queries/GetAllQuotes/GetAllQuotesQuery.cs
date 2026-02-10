using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Queries.GetAllQuotes;

public record GetAllQuotesQuery(int PageNumber, int PageSize) : IRequest<PagedResult<QuoteResponse>>;
