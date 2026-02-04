using EBOS.CRM.Domain.Entities.Identity;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories.Identity;

public class UserEntityFactoryTest
{
    public static User CreateValidUser(
        string externalId = "ext-001",
        string username = "jdoe",
        string email = "jdoe@example.com",
        string displayName = "John Doe",
        bool isActive = true)
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
        var entity = CreateValidUser();

        Assert.NotNull(entity);
        Assert.Equal("ext-001", entity.ExternalId);
        Assert.Equal("jdoe", entity.Username);
        Assert.Equal("jdoe@example.com", entity.Email);
        Assert.Equal("John Doe", entity.DisplayName);
        Assert.True(entity.IsActive);
    }

    [Fact]
    public void CreateValidUser_CustomValues_AreApplied()
    {
        var entity = CreateValidUser(
            externalId: "ext-002",
            username: "asmith",
            email: "asmith@example.com",
            displayName: "Alice Smith",
            isActive: false);

        Assert.Equal("ext-002", entity.ExternalId);
        Assert.Equal("asmith", entity.Username);
        Assert.Equal("asmith@example.com", entity.Email);
        Assert.Equal("Alice Smith", entity.DisplayName);
        Assert.False(entity.IsActive);
    }
}
