

using MediatR;


namespace EBOS.CRM.Application.Features.CRM.BankInformation.Commands.DeleteBankInformation;

public record DeleteBankInformationCommand(long Id) : IRequest<bool>;




