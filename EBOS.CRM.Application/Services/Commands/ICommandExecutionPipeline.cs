using EBOS.Core.Primitives.Interfaces;

namespace EBOS.CRM.Application.Services.Commands;

public interface ICommandExecutionPipeline
{
    Task<TResponse> ExecuteAsync<TResponse>(
        IUnitOfWork unitOfWork,
        Func<CancellationToken, Task<CommandExecutionResult<TResponse>>> operation,
        CancellationToken cancellationToken = default);
}
