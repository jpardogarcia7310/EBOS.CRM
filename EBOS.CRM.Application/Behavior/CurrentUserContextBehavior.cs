using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Behavior;

public sealed class CurrentUserContextBehavior<TRequest, TResponse>(ICurrentUserContext currentUser)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ICurrentUserContext _currentUser = currentUser
                                                        ?? throw new ArgumentNullException(nameof(currentUser));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (RequiresUserContext(request) && _currentUser.UserId <= 0)
        {
            throw new UnauthorizedAccessException("Current user context is required.");
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }

    private static bool RequiresUserContext(TRequest request)
    {
        var requestType = request.GetType();
        var ns = requestType.Namespace ?? string.Empty;

        if (request is IAllowAnonymousRequest)
        {
            return false;
        }

        return !ns.Contains(".Features.Security.", StringComparison.Ordinal) &&
               ns.Contains(".Commands.", StringComparison.Ordinal);
    }
}
