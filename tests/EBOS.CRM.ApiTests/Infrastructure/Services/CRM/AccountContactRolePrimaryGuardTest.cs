using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Services.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.CRM;

public class AccountContactRolePrimaryGuardTest
{
    [Fact]
    public async Task GetOtherPrimariesAsync_FiltersByPrimaryAndExclusion()
    {
        var repositoryMock = new Mock<IAccountContactRoleRepository>();
        var guard = new AccountContactRolePrimaryGuard(repositoryMock.Object);

        var roles = new List<AccountContactRole>
        {
            AccountContactRole.Create(1, 10, "BILLING", true, DateTime.UtcNow.AddDays(-2), null),
            AccountContactRole.Create(1, 10, "LEGAL", true, DateTime.UtcNow.AddDays(-2), null),
            AccountContactRole.Create(1, 10, "TECH", false, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1))
        };
        SetEntityId(roles[0], 1);
        SetEntityId(roles[1], 2);
        SetEntityId(roles[2], 3);

        repositoryMock
            .Setup(r => r.GetByAccountContactPagedAsync(1, 10, 1, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        var result = await guard.GetOtherPrimariesAsync(1, 10, 1, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(2, result.First().Id);
        repositoryMock.Verify(r => r.GetByAccountContactPagedAsync(1, 10, 1, int.MaxValue,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static void SetEntityId(AccountContactRole role, long id)
    {
        var idProperty = role.GetType().BaseType?.GetProperty("Id");
        idProperty?.SetValue(role, id);
    }
}
