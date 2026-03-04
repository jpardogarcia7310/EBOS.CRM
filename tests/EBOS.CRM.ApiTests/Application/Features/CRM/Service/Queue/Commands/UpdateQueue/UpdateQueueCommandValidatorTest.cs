using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.UpdateQueue;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Queue.Commands.UpdateQueue;

public class UpdateQueueCommandValidatorTest
{
    private readonly UpdateQueueCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new UpdateQueueCommand(1, new UpdateQueueRequest(1, 1, "Q", "Q1", true, null)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new UpdateQueueCommand(1, null!));
        Assert.False(result.IsValid);
    }
}
