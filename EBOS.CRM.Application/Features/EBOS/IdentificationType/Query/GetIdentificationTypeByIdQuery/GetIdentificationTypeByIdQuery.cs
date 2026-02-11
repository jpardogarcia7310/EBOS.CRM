using EBOS.CRM.Application.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetIdentificationTypeByIdQuery;

public record GetIdentificationTypeByIdQuery(long Id) : IRequest<IdentificationTypeResponse?>;



