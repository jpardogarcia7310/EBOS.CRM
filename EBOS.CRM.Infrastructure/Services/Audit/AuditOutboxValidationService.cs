using System.Text.Json;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;

namespace EBOS.CRM.Infrastructure.Services.Audit;

public sealed class AuditOutboxValidationService : IAuditOutboxValidationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public void EnsureEnqueueRequestIsValid(string operation, AuditInsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            throw new DomainValidationException("Operation is required.", "DOMAIN_VALIDATION_AUDIT_OPERATION_REQUIRED");
        }

        if (request.UserId <= 0)
        {
            throw new DomainValidationException("Audit user id must be positive.", "DOMAIN_VALIDATION_AUDIT_USER_ID_POSITIVE");
        }

        if (string.IsNullOrWhiteSpace(request.CorrelationId))
        {
            throw new DomainValidationException("CorrelationId is required.", "DOMAIN_VALIDATION_AUDIT_CORRELATION_REQUIRED");
        }
    }

    public AuditInsertRequest EnsureDispatchPayloadIsValid(AuditOutboxMessage message)
    {
        var request = JsonSerializer.Deserialize<AuditInsertRequest>(message.Payload, JsonOptions);
        if (request is null)
        {
            throw new DomainValidationException("Invalid outbox payload.", "DOMAIN_VALIDATION_AUDIT_OUTBOX_PAYLOAD_INVALID");
        }

        EnsureEnqueueRequestIsValid(message.Operation, request);
        return request;
    }
}
