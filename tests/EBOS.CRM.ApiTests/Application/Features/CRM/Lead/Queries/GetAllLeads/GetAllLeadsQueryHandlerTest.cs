using EBOS.CRM.Application.Features.CRM.Lead.Queries.GetAllLeads;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Queries.GetAllLeads;

public class GetAllLeadsQueryHandlerTest
{
    private readonly Mock<ILeadRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.Lead> { new() { Id = 1, TenantId = 1, Source = "WEB", Status = "NEW", OwnerUserId = 1, CompanyName = "ACME", ContactName = "John", Email = "john@acme.com", Phone = "111" } };
        _repository.Setup(x => x.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        _repository.Setup(x => x.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities.Count);
        _mapper.Setup(x => x.Map<IReadOnlyCollection<LeadResponse>>(entities)).Returns(new List<LeadResponse>());

        var handler = new GetAllLeadsQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetAllLeadsQuery(), CancellationToken.None);

        Assert.NotNull(result);
        _repository.Verify(x => x.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetAllLeadsQueryHandler(_repository.Object, _mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllLeadsQuery(), cts.Token));
    }
}
