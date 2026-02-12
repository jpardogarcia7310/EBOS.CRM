namespace EBOS.CRM.Application.Shared.Commands;

public sealed class CommandConcurrencyException(string message, Exception? innerException = null)
    : Exception(message, innerException)
{
}
