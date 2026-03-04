using EBOS.CRM.Application.Features.CRM.Lead.Queries.GetLeadById;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Queries.GetLeadById;

public class GetLeadByIdQueryHandlerTest
{
    private readonly Mock<ILeadRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task Handle_WhenExists_ReturnsMappedResponse()
    {
        var entity = new global::EBOS.CRM.Domain.Entities.CRM.Lead { Id = 7, TenantId = 1, Source = "WEB", Status = "NEW", OwnerUserId = 5, CompanyName = "ACME", ContactName = "John", Email = "john@acme.com", Phone = "111" };
        _repository.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _mapper.Setup(x => x.Map<LeadResponse>(entity))
            .Returns(new LeadResponse(7, 1, "WEB", "NEW", 5, "ACME", "John", "john@acme.com", "111", null, null, null, true));

        var handler = new GetLeadByIdQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetLeadByIdQuery(7), CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        _repository.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Lead?)null);
        var handler = new GetLeadByIdQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetLeadByIdQuery(99), CancellationToken.None);
        Assert.Null(result);
    }
}
