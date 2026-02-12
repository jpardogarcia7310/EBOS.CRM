using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetAllTaxInformationAddresses;

public record GetAllTaxInformationAddressesQuery(int PageNumber = 1, int PageSize = 50) :
    IRequest<PagedResult<TaxInformationAddressResponse>>;









