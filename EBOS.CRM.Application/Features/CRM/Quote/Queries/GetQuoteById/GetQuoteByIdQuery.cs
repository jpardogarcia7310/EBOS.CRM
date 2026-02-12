using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Queries.GetQuoteById;

public record GetQuoteByIdQuery(long Id) : IRequest<QuoteResponse?>;
