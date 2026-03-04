using EBOS.CRM.Application.Features.EBOS.ValidationRules.Queries.GetAllValidationRules;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ValidationRules.Queries.GetAllValidationRules;

public class GetAllValidationRulesQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repository = new Mock<IValidationRuleRepository>();
        var mapper = new Mock<IMapper>();
        var entities = new List<ValidationRule> { new() { Id = 1, Key = "email", Pattern = ".+@.+", IsActive = true } };
        repository.Setup(x => x.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        repository.Setup(x => x.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(x => x.Map<IReadOnlyCollection<ValidationRuleResponse>>(entities))
            .Returns(new List<ValidationRuleResponse> { new(1, "email", ".+@.+", null, true) });

        var handler = new GetAllValidationRulesQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetAllValidationRulesQuery(1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}
