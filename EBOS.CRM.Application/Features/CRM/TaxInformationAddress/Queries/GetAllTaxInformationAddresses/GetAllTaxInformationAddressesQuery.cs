using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetAllTaxInformationAddresses;

public record GetAllTaxInformationAddressesQuery : IRequest<IReadOnlyCollection<TaxInformationAddressResponse>>;









