using System;

namespace EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;

public record UpdateCreditTransactionRequest(
    long TenantId,
    DateTime Date,
    decimal Amount,
    string Type,
    string ExternalReference,
    string Comments,
    long CreditAccountId
);
