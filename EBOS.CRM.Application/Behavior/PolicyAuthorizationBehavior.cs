using EBOS.CRM.Application.Services.Authorization;
using EBOS.CRM.Application.Services.Interfaces;
using MediatR;

namespace EBOS.CRM.Application.Behavior;

public sealed class PolicyAuthorizationBehavior<TRequest, TResponse>(ICurrentUserContext currentUser,
    IPolicyService policyService) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ICurrentUserContext _currentUser = currentUser
        ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IPolicyService _policyService = policyService
        ?? throw new ArgumentNullException(nameof(policyService));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var policyCode = PolicyCodeResolver.Resolve(request.GetType());
        if (!string.IsNullOrWhiteSpace(policyCode) && _currentUser.UserId > 0)
        {
            await _policyService.EnsureAuthorizedAsync(_currentUser.UserId, policyCode, cancellationToken);
        }

        return await next(cancellationToken);
    }
}
