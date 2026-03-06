using System.Net.Http;

namespace EBOS.CRM.Domain.Exceptions;

public static class DomainTransientFailureClassifier
{
    public static bool TryClassify(
        Exception exception,
        string operation,
        out TransientDomainFailureException transient)
    {
        if (exception is DomainException)
        {
            transient = null!;
            return false;
        }

        var root = Unwrap(exception);
        var message = root.Message ?? string.Empty;

        if (root is TimeoutException || root is TaskCanceledException)
        {
            transient = new TransientDomainFailureException(
                $"Transient timeout while executing {operation}.",
                "DOMAIN_TRANSIENT_TIMEOUT",
                exception);
            return true;
        }

        if (root is OperationCanceledException)
        {
            transient = null!;
            return false;
        }

        if (root is HttpRequestException)
        {
            transient = new TransientDomainFailureException(
                $"Transient dependency failure while executing {operation}.",
                "DOMAIN_TRANSIENT_DEPENDENCY_UNAVAILABLE",
                exception);
            return true;
        }

        if (LooksLikeTransientPersistence(root, message))
        {
            transient = new TransientDomainFailureException(
                $"Transient persistence failure while executing {operation}.",
                "DOMAIN_TRANSIENT_PERSISTENCE",
                exception);
            return true;
        }

        transient = null!;
        return false;
    }

    private static Exception Unwrap(Exception ex)
    {
        while (ex.InnerException is not null)
        {
            ex = ex.InnerException;
        }

        return ex;
    }

    private static bool LooksLikeTransientPersistence(Exception root, string message)
    {
        var typeName = root.GetType().Name;
        if (typeName.Contains("DbUpdateException", StringComparison.Ordinal) ||
            typeName.Contains("SqlException", StringComparison.Ordinal))
        {
            return true;
        }

        return message.Contains("deadlock", StringComparison.OrdinalIgnoreCase)
               || message.Contains("timeout", StringComparison.OrdinalIgnoreCase)
               || message.Contains("connection", StringComparison.OrdinalIgnoreCase)
               || message.Contains("transport-level", StringComparison.OrdinalIgnoreCase);
    }
}
