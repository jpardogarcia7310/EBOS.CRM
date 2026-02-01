using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType;

public record GetAllIdentificationTypeQuery(PagedQueryRequest Query) : IRequest<PagedResponse<IdentificationTypeResponse>>;




