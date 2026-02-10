using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Quote.Queries.GetAllQuotes;

public class GetAllQuotesQueryHandler(IQuoteRepository repository, IMapper mapper)
    : IRequestHandler<GetAllQuotesQuery, PagedResult<QuoteResponse>>
{
    private readonly IQuoteRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<QuoteResponse>> Handle(GetAllQuotesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<QuoteResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<QuoteResponse>(items, total);
    }
}
