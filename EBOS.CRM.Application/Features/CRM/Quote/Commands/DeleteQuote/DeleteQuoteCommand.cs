using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Commands.DeleteQuote;

public record DeleteQuoteCommand(long Id) : IRequest<bool>;
