using EBOS.CRM.Application.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.AddQuote;

public record AddQuoteCommand(AddQuoteRequest QuoteRequest) : IRequest<QuoteResponse>;
