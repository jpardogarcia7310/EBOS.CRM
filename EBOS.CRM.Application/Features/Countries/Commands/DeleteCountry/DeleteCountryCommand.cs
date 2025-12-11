using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Commands.DeleteCountry;

public sealed record DeleteCountryCommand(long Id) : IRequest<Unit>;