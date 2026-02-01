using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetAllCreditTransactions;

public class GetAllCreditTransactionsQueryHandler(ICreditTransactionRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCreditTransactionsQuery, PagedResponse<CreditTransactionResponse>>
{
    private readonly ICreditTransactionRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<CreditTransactionResponse>> Handle(GetAllCreditTransactionsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CreditTransactionResponse>>(result.Items);
        return new PagedResponse<CreditTransactionResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




