using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;

public class GetAllCreditAccountsQueryHandler(ICreditAccountRepository repository, IMapper mapper)
    : IRequestHandler<GetAllCreditAccountsQuery, PagedResponse<CreditAccountResponse>>
{
    private readonly ICreditAccountRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<CreditAccountResponse>> Handle(GetAllCreditAccountsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<CreditAccountResponse>>(result.Items);
        return new PagedResponse<CreditAccountResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




