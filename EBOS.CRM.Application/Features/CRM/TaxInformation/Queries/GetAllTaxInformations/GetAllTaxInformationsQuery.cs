using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformations;

public record GetAllTaxInformationsQuery : IRequest<IReadOnlyCollection<TaxInformationResponse>>;









