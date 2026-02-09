namespace EBOS.CRM.Application.Services.Commands;

public sealed class CommandConcurrencyException(string message, Exception? innerException = null)
    : Exception(message, innerException)
{
}
