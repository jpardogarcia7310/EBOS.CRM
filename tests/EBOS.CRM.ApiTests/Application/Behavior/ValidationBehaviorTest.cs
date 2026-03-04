using EBOS.CRM.Application.Behavior;
using FluentValidation;
using MediatR;

namespace EBOS.CRM.ApiTests.Application.Behavior;

public class ValidationBehaviorTest
{
    [Fact]
    public async Task Handle_WhenNoValidators_InvokesNext()
    {
        var sut = new ValidationBehavior<TestRequest, string>(Array.Empty<IValidator<TestRequest>>());

        var result = await sut.Handle(new TestRequest("ok"), _ => Task.FromResult("done"), CancellationToken.None);

        Assert.Equal("done", result);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ThrowsValidationException()
    {
        var validators = new IValidator<TestRequest>[] { new TestRequestValidator() };
        var sut = new ValidationBehavior<TestRequest, Unit>(validators);

        var act = () => sut.Handle(new TestRequest(string.Empty), _ => Task.FromResult(Unit.Value), CancellationToken.None);

        await Assert.ThrowsAsync<ValidationException>(act);
    }

    private sealed record TestRequest(string Name);

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
