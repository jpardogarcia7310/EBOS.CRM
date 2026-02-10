using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class UserEntityFactoryTest
{
    private static User CreateValidUser(string externalId = "ext-123", string username = "jdoe",
        string email = "jdoe@example.com", string displayName = "John Doe", bool isActive = true)
    {
        return new User
        {
            ExternalId = externalId,
            Username = username,
            Email = email,
            DisplayName = displayName,
            IsActive = isActive
        };
    }

    [Fact]
    public void CreateValidUser_Defaults_AreSet()
    {
        var user = CreateValidUser();

        Assert.NotNull(user);
        Assert.Equal("ext-123", user.ExternalId);
        Assert.Equal("jdoe", user.Username);
        Assert.Equal("jdoe@example.com", user.Email);
        Assert.Equal("John Doe", user.DisplayName);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void CreateValidUser_CustomValues_AreApplied()
    {
        var user = CreateValidUser(externalId: "ext-456", username: "asmith", email: "asmith@example.com",
            displayName: "Alice Smith", isActive: false);

        Assert.Equal("ext-456", user.ExternalId);
        Assert.Equal("asmith", user.Username);
        Assert.Equal("asmith@example.com", user.Email);
        Assert.Equal("Alice Smith", user.DisplayName);
        Assert.False(user.IsActive);
    }
}
