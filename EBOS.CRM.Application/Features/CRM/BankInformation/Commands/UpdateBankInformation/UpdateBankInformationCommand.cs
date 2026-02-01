using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Commands.UpdateBankInformation;

public record UpdateBankInformationCommand(long Id, UpdateBankInformationRequest BankInformationRequest) : IRequest<BankInformationResponse?>;
