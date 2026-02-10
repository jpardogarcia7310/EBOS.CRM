using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Queries.GetAllQuotes;

public class GetAllQuotesQueryHandler(IQuoteRepository repository, IMapper mapper)
    : IRequestHandler<GetAllQuotesQuery, PagedResult<QuoteResponse>>
{
    public async Task<PagedResult<QuoteResponse>> Handle(GetAllQuotesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = mapper.Map<IReadOnlyCollection<QuoteResponse>>(entities);
        var total = await repository.CountAsync(cancellationToken);
        return new PagedResult<QuoteResponse>(items, total);
    }
}
