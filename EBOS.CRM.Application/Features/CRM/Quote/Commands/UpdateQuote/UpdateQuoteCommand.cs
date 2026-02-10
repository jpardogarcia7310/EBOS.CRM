using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.UpdateQuote;

public record UpdateQuoteCommand(long Id, UpdateQuoteRequest QuoteRequest) : IRequest<QuoteResponse?>;
