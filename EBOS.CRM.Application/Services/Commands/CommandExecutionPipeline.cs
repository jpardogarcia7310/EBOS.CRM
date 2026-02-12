using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Application.Services.Commands;

public sealed class CommandExecutionPipeline(
    IAuditService auditService,
    IOptions<CommandExecutionOptions> options)
    : ICommandExecutionPipeline
{
    private readonly CommandExecutionOptions _options = options.Value;

    public async Task<TResponse> ExecuteAsync<TResponse>(
        IUnitOfWork unitOfWork,
        Func<CancellationToken, Task<CommandExecutionResult<TResponse>>> operation,
        CancellationToken cancellationToken = default)
    {
        var retries = Math.Max(1, _options.ConcurrencyRetryCount);

        for (var attempt = 1; attempt <= retries; attempt++)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await operation(cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);

                var auditRequest = result.BuildAuditRequest?.Invoke();
                if (auditRequest != null)
                {
                    await auditService.InsertAuditAsync(auditRequest, cancellationToken);
                }

                return result.Response;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                if (attempt >= retries)
                {
                    throw new CommandConcurrencyException(
                        "Command failed due to concurrent updates after retries.",
                        ex);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(_options.ConcurrencyRetryDelayMs * attempt), cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        throw new CommandConcurrencyException("Command failed due to concurrent updates after retries.");
    }
}
