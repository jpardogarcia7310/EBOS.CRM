using EBOS.CRM.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.UpdateQuote;

public record UpdateQuoteCommand(long Id, UpdateQuoteRequest QuoteRequest) : IRequest<QuoteResponse?>;
