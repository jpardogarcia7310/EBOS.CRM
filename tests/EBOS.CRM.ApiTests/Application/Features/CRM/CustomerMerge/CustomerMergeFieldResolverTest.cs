using EBOS.CRM.Application.Features.CRM.CustomerMerge;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge;

public class CustomerMergeFieldResolverTest
{
    [Fact]
    public void ResolveString_WhenCandidateNewer_ReturnsCandidate()
    {
        var sut = BuildResolver();

        var value = sut.ResolveString("winner", DateTime.UtcNow.AddMinutes(-10), "candidate", DateTime.UtcNow, preferWinner: true);

        Assert.Equal("candidate", value);
    }

    [Fact]
    public void ResolveString_WhenTieAndPreferWinnerFalse_ReturnsCandidateTrimmed()
    {
        var sut = BuildResolver(new CustomerMergeOptions { MaxFieldLength = 3, PreferWinnerOnTie = false });
        var now = DateTime.UtcNow;

        var value = sut.ResolveString("win", now, "candidate", now, preferWinner: false);

        Assert.Equal("can", value);
    }

    [Fact]
    public void ResolveLong_WhenWinnerInvalid_ReturnsCandidate()
    {
        var sut = BuildResolver();

        var value = sut.ResolveLong(0, DateTime.UtcNow, 25, DateTime.UtcNow, preferWinner: true);

        Assert.Equal(25, value);
    }

    [Fact]
    public void ResolveDate_WhenCandidateDefault_ReturnsWinner()
    {
        var sut = BuildResolver();
        var winner = new DateTime(2020, 1, 1);

        var value = sut.ResolveDate(winner, DateTime.UtcNow, default, DateTime.UtcNow, preferWinner: true);

        Assert.Equal(winner, value);
    }

    [Fact]
    public void ResolveUpdatedBy_WhenNull_ReturnsCurrentUser()
    {
        var sut = BuildResolver(currentUserId: 55);

        var value = sut.ResolveUpdatedBy(null);

        Assert.Equal(55, value);
    }

    [Fact]
    public void FieldContext_Record_HoldsValues()
    {
        var ctx = new CustomerMergeFieldContext("CRM", "Email", "High");

        Assert.Equal("CRM", ctx.Source);
        Assert.Equal("Email", ctx.ChannelKey);
        Assert.Equal("High", ctx.Confidentiality);
    }

    private static CustomerMergeFieldResolver BuildResolver(CustomerMergeOptions? options = null, long currentUserId = 10)
    {
        var user = new Mock<ICurrentUserContext>();
        user.SetupGet(x => x.UserId).Returns(currentUserId);

        return new CustomerMergeFieldResolver(user.Object, options ?? new CustomerMergeOptions());
    }
}
