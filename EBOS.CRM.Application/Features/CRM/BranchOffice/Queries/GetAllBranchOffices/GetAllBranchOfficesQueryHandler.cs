using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;

public class GetAllBranchOfficesQueryHandler(IBranchOfficeRepository repository, IMapper mapper)
    : IRequestHandler<GetAllBranchOfficesQuery, IReadOnlyCollection<BranchOfficeResponse>>
{
    private readonly IBranchOfficeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<IReadOnlyCollection<BranchOfficeResponse>> Handle(GetAllBranchOfficesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyCollection<BranchOfficeResponse>>(entities);
    }
}









