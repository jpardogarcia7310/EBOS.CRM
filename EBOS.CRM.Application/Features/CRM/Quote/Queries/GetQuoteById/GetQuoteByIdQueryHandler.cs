using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Queries.GetQuoteById;

public class GetQuoteByIdQueryHandler(IQuoteRepository repository, IMapper mapper)
    : IRequestHandler<GetQuoteByIdQuery, QuoteResponse?>
{
    public async Task<QuoteResponse?> Handle(GetQuoteByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : mapper.Map<QuoteResponse>(entity);
    }
}
