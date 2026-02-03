using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformation;

public sealed record GetAllTaxInformationQuery() : IRequest<IReadOnlyCollection<TaxInformationResponse>>;





