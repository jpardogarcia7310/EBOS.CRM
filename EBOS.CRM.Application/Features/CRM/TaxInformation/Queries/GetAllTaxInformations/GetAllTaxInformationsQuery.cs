using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformations;

public record GetAllTaxInformationsQuery(PagedQueryRequest Query) : IRequest<PagedResponse<TaxInformationResponse>>;




