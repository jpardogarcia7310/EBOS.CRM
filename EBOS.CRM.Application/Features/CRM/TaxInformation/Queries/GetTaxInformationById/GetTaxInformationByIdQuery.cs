using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetTaxInformationById;

public record GetTaxInformationByIdQuery(long Id) : IRequest<TaxInformationResponse?>;




