using EBOS.CRM.Application.Contracts.Responses;
using MediatR;

namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeById;

public abstract record GetIdentificationTypeQuery(long Id) : IRequest<IdentificationTypeResponse?>;