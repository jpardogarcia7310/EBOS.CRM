using EBOS.CRM.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.AddQuote;

public record AddQuoteCommand(AddQuoteRequest QuoteRequest) : IRequest<QuoteResponse>;
