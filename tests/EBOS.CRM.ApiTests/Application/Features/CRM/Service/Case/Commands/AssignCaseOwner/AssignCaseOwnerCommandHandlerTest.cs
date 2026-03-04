using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseOwner;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.AssignCaseOwner;

public class AssignCaseOwnerCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenCaseNotFound_ReturnsNull()
    {
        var repo = new Mock<ICaseRepository>();
        repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((CaseEntity?)null);

        var handler = new AssignCaseOwnerCommandHandler(
            repo.Object,
            new Mock<IAuditService>().Object,
            AssignCaseOwnerTestHelper.BuildCurrentUser().Object,
            new Mock<IMapper>().Object);

        var result = await handler.Handle(new AssignCaseOwnerCommand(99, new AssignCaseOwnerRequest(1, 5)), CancellationToken.None);

        Assert.Null(result);
    }
}
