using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetTaxInformationAddressById;

public record GetTaxInformationAddressByIdQuery(long Id) : IRequest<TaxInformationAddressResponse?>;




