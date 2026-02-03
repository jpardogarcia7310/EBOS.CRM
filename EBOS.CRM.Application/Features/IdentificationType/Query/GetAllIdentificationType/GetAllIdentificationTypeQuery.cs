using EBOS.CRM.Application.Contracts.Responses;
using MediatR;


namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType;

public record GetAllIdentificationTypeQuery : IRequest<IReadOnlyCollection<IdentificationTypeResponse>>;









