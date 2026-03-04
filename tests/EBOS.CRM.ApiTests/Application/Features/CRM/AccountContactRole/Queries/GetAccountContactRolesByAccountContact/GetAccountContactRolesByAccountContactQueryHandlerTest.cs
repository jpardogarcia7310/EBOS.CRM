using EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRolesByAccountContact;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMAccountContactRole = EBOS.CRM.Domain.Entities.CRM.AccountContactRole;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRolesByAccountContact;

public class GetAccountContactRolesByAccountContactQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repository = new Mock<IAccountContactRoleRepository>();
        var mapper = new Mock<IMapper>();

        var roles = new List<CRMAccountContactRole> { CRMAccountContactRole.Create(1, 10, "OWNER", false, DateTime.UtcNow, null) };
        repository.Setup(x => x.GetByAccountContactPagedAsync(1, 10, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
        repository.Setup(x => x.CountByAccountContactAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        mapper.Setup(x => x.Map<IReadOnlyCollection<AccountContactRoleResponse>>(roles))
            .Returns(new[] { new AccountContactRoleResponse(1, 1, 10, "OWNER", false, DateTime.UtcNow, null, true) });

        var handler = new GetAccountContactRolesByAccountContactQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetAccountContactRolesByAccountContactQuery(1, 10, 1, 10),
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
    }
}
