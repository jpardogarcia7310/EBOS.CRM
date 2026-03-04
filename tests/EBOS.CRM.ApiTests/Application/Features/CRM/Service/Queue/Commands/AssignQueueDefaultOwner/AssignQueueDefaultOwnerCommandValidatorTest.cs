using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AssignQueueDefaultOwner;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Queue.Commands.AssignQueueDefaultOwner;

public class AssignQueueDefaultOwnerCommandValidatorTest
{
    private readonly AssignQueueDefaultOwnerCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new AssignQueueDefaultOwnerCommand(1, new AssignQueueDefaultOwnerRequest(1, 123)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidId_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new AssignQueueDefaultOwnerCommand(0, new AssignQueueDefaultOwnerRequest(1, 123)));
        Assert.False(result.IsValid);
    }
}
