using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;

public sealed record PatchTaxInformationCommand(long Id, PatchTaxInformationRequest TaxInformationRequest)
    : IRequest<TaxInformationResponse?>;
