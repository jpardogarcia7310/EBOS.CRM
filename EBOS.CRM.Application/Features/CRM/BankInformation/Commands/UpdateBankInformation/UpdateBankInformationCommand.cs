using EBOS.CRM.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Commands.UpdateBankInformation;

public record UpdateBankInformationCommand(long Id, UpdateBankInformationRequest BankInformationRequest) :
    IRequest<BankInformationResponse?>;




