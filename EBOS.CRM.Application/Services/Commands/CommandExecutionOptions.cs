namespace EBOS.CRM.Application.Services.Commands;

public sealed class CommandExecutionOptions
{
    public const string SectionName = "CommandExecution";

    public int ConcurrencyRetryCount { get; set; } = 3;

    public int ConcurrencyRetryDelayMs { get; set; } = 200;
}
