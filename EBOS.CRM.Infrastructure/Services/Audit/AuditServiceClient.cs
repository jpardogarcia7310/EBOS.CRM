using System.Net.Http.Json;
using System.Text.Json;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.Models;
using AuditServiceUnavailableException = EBOS.CRM.Application.Services.Audit.AuditServiceUnavailableException;

namespace EBOS.CRM.Infrastructure.Services.Audit;

public sealed class AuditServiceClient(HttpClient httpClient, IOptions<AuditServiceOptions> options)
    : IAuditService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly AuditServiceOptions _options = options.Value;

    public async Task<AuditInsertResponse> InsertAuditAsync(
        AuditInsertRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new AuditInsertResponse(true, 0);
        }

        var endpoint = "api/audit/InsertAudit";
        return await ExecuteWithRetryAsync(
            () => httpClient.PostAsJsonAsync(endpoint, request, JsonOptions, cancellationToken),
            async response =>
            {
                var payload = await response.Content.ReadFromJsonAsync<AuditInsertResponse>(
                    JsonOptions, cancellationToken);
                return payload ?? new AuditInsertResponse(true, 0);
            },
            "InsertAudit",
            cancellationToken);
    }

    public Task<IReadOnlyCollection<AuditRecord>> GetAllByEntityAsync(
        string entity,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return Task.FromResult<IReadOnlyCollection<AuditRecord>>(Array.Empty<AuditRecord>());
        }

        var endpoint = $"api/audit/GetAllByEntity?entity={Uri.EscapeDataString(entity)}";
        return GetListAsync(endpoint, "GetAllByEntity", cancellationToken);
    }

    public Task<IReadOnlyCollection<AuditRecord>> GetAllByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return Task.FromResult<IReadOnlyCollection<AuditRecord>>(Array.Empty<AuditRecord>());
        }

        var endpoint = $"api/audit/GetAllByUserId?userId={userId}";
        return GetListAsync(endpoint, "GetAllByUserId", cancellationToken);
    }

    public Task<IReadOnlyCollection<AuditRecord>> GetAllByRegisterIdAsync(
        long registerId,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return Task.FromResult<IReadOnlyCollection<AuditRecord>>(Array.Empty<AuditRecord>());
        }

        var endpoint = $"api/audit/GetAllByRegisterId?registerId={registerId}";
        return GetListAsync(endpoint, "GetAllByRegisterId", cancellationToken);
    }

    private Task<IReadOnlyCollection<AuditRecord>> GetListAsync(
        string endpoint,
        string operationName,
        CancellationToken cancellationToken)
    {
        return ExecuteWithRetryAsync(
            () => httpClient.GetAsync(endpoint, cancellationToken),
            async response =>
            {
                var payload = await response.Content.ReadFromJsonAsync<List<AuditRecord>>(
                    JsonOptions, cancellationToken);
                return (IReadOnlyCollection<AuditRecord>)(payload ?? []);
            },
            operationName,
            cancellationToken);
    }

    private async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<HttpResponseMessage>> action,
        Func<HttpResponseMessage, Task<T>> onSuccess,
        string operationName,
        CancellationToken cancellationToken)
    {
        Exception? lastException = null;
        var retries = Math.Max(1, _options.RetryCount);

        for (var attempt = 1; attempt <= retries; attempt++)
        {
            try
            {
                var response = await action();
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    lastException = new AuditServiceUnavailableException(
                        $"Audit service error on {operationName}. Status: {(int)response.StatusCode}. Body: {content}");
                }
                else
                {
                    return await onSuccess(response);
                }
            }
            catch (Exception ex) when (ex is not AuditServiceUnavailableException)
            {
                lastException = ex;
            }

            if (attempt < retries)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt), cancellationToken);
            }
        }

        throw lastException is AuditServiceUnavailableException
            ? lastException
            : new AuditServiceUnavailableException(
                $"Audit service unavailable after {retries} attempts during {operationName}.",
                lastException);
    }
}