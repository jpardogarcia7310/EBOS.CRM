using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AddQueue;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Queue.Commands.AddQueue;

public class AddQueueCommandValidatorTest
{
    private readonly AddQueueCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new AddQueueCommand(new AddQueueRequest(1, "Default", "DEF", true, null)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new AddQueueCommand(null!));
        Assert.False(result.IsValid);
    }
}
