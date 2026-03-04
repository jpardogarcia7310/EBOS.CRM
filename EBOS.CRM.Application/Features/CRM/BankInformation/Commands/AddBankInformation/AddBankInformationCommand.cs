using EBOS.CRM.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Commands.AddBankInformation;

public record AddBankInformationCommand(AddBankInformationRequest BankInformationRequest) :
    IRequest<BankInformationResponse>;




