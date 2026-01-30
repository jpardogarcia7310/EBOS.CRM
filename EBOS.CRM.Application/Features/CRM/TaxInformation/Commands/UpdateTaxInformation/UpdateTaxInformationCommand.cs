using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;

public sealed record UpdateTaxInformationCommand(UpdateTaxInformationRequest TaxInformationRequest)
    : IRequest<TaxInformationResponse?>;
