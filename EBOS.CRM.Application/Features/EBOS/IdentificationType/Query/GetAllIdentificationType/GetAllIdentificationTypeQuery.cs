using MediatR;
using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.EBOS;

namespace EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetAllIdentificationType;

public record GetAllIdentificationTypeQuery(int PageNumber = 1, int PageSize = 50) :
    IRequest<PagedResult<IdentificationTypeResponse>>;









