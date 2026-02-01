using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetAllTaxInformationAddresses;

public record GetAllTaxInformationAddressesQuery(PagedQueryRequest Query) : IRequest<PagedResponse<TaxInformationAddressResponse>>;




