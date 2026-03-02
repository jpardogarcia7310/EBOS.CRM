using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Services.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.CRM;

public class AccountContactPrimaryGuardTest
{
    [Fact]
    public async Task GetOtherPrimariesAsync_FiltersByPrimaryAndExclusion()
    {
        var repositoryMock = new Mock<IAccountContactRepository>();
        var guard = new AccountContactPrimaryGuard(repositoryMock.Object);

        var contacts = new List<AccountContact>
        {
            AccountContact.Create(1, 10, 100, true, DateTime.UtcNow, null, 1),
            AccountContact.Create(1, 10, 101, true, DateTime.UtcNow, null, 1),
            AccountContact.Create(1, 10, 102, false, DateTime.UtcNow, null, 1)
        };

        contacts[0].Id = 1;
        contacts[1].Id = 2;
        contacts[2].Id = 3;

        repositoryMock
            .Setup(r => r.GetByCorporateCustomerPagedAsync(1, 10, 1, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contacts);

        var result = await guard.GetOtherPrimariesAsync(1, 10, 1, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(2, result.First().Id);
        repositoryMock.Verify(r => r.GetByCorporateCustomerPagedAsync(1, 10, 1, int.MaxValue,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
