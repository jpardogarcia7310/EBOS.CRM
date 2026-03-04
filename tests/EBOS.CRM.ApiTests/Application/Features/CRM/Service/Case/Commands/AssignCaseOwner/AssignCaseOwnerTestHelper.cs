using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.AssignCaseOwner;

internal static class AssignCaseOwnerTestHelper
{
    internal static Mock<ICurrentUserContext> BuildCurrentUser()
    {
        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr");
        return currentUser;
    }
}
