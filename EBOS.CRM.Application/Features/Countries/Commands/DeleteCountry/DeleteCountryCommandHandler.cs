using EBOS.CRM.Domain.Interfaces.Repositories;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Commands.DeleteCountry;

public sealed class DeleteCountryCommandHandler(ICountryRepository repository) : IRequestHandler<DeleteCountryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        _ = await repository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException($"Country with id {request.Id} not found.");

        await repository.DeleteAsync(request.Id, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}