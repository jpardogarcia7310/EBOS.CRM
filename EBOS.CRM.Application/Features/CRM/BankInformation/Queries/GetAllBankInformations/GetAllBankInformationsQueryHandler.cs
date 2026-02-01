using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;

public class GetAllBankInformationsQueryHandler(IBankInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBankInformationsQuery, PagedResponse<BankInformationResponse>>
{
    private readonly IBankInformationRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResponse<BankInformationResponse>> Handle(GetAllBankInformationsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _repository.GetPagedAsync(request.Query.ToPagedQuery(), cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<BankInformationResponse>>(result.Items);
        return new PagedResponse<BankInformationResponse>(items, result.PageNumber, result.PageSize, result.TotalCount, result.TotalPages, result.SortBy, result.SortDirection, result.Filter);
    }
}




