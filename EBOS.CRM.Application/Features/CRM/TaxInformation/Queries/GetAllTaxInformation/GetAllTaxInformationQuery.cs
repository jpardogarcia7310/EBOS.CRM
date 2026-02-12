using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformation;

public sealed record GetAllTaxInformationQuery(int PageNumber = 1, int PageSize = 50) :
    IRequest<PagedResult<TaxInformationResponse>>;





