using EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetValidationRuleById;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ValidationRules.Queries.GetValidationRuleById;

public class GetValidationRuleByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenExists_ReturnsDto()
    {
        var repository = new Mock<IValidationRuleRepository>();
        var mapper = new Mock<IMapper>();
        var entity = new ValidationRule { Id = 1, Key = "email", Pattern = ".+@.+", IsActive = true };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(x => x.Map<ValidationRuleResponse>(entity))
            .Returns(new ValidationRuleResponse(1, "email", ".+@.+", null, true));

        var handler = new GetValidationRuleByIdQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetValidationRuleByIdQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ThrowsKeyNotFound()
    {
        var repository = new Mock<IValidationRuleRepository>();
        var mapper = new Mock<IMapper>();
        repository.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((ValidationRule?)null);

        var handler = new GetValidationRuleByIdQueryHandler(repository.Object, mapper.Object);
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new GetValidationRuleByIdQuery(99), CancellationToken.None));
    }
}
