using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.ToggleQueue;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Queue.Commands.ToggleQueue;

public class ToggleQueueCommandValidatorTest
{
    private readonly ToggleQueueCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new ToggleQueueCommand(1, new ToggleQueueRequest(1, false)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new ToggleQueueCommand(0, new ToggleQueueRequest(1, false)));
        Assert.False(result.IsValid);
    }
}
