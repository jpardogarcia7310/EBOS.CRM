using EBOS.CRM.Application.Contracts.Responses;
using MediatR;

namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery;

public record GetIdentificationTypeByIdQuery(long Id) : IRequest<IdentificationTypeResponse?>;



