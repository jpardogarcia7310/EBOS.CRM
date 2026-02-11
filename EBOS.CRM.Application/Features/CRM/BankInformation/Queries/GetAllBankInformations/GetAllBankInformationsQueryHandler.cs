using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;

public class GetAllBankInformationsQueryHandler(IBankInformationRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBankInformationsQuery, PagedResult<BankInformationResponse>>
{
    private readonly IBankInformationRepository _repository = repository ??
                                                              throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<PagedResult<BankInformationResponse>> Handle(GetAllBankInformationsQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllPagedAsync(request.PageNumber,
            request.PageSize, cancellationToken);
        var items = _mapper.Map<IReadOnlyCollection<BankInformationResponse>>(entities);
        var total = await _repository.CountAsync(cancellationToken);
        return new PagedResult<BankInformationResponse>(items, total);
    }
}










