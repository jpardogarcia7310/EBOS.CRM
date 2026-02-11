using System.Collections;
using System.Linq.Expressions;
using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;

public class CheckSlaBatchQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenCasesExist_ReturnsPagedSlaChecks()
    {
        var now = new DateTime(2026, 2, 11, 12, 0, 0, DateTimeKind.Utc);
        var cases = new List<Case>
        {
            new()
            {
                Id = 1,
                TenantId = 1,
                Status = Case.StatusOpen,
                Priority = Case.PriorityLow,
                SlaId = 10,
                DueAt = now.AddMinutes(-5)
            },
            new()
            {
                Id = 2,
                TenantId = 1,
                Status = Case.StatusClosed,
                Priority = Case.PriorityLow,
                SlaId = 10,
                DueAt = now.AddMinutes(10)
            },
            new()
            {
                Id = 3,
                TenantId = 2,
                Status = Case.StatusOpen,
                Priority = Case.PriorityLow,
                SlaId = 11,
                DueAt = now.AddMinutes(20)
            }
        };

        var slas = new List<Sla>
        {
            new()
            {
                Id = 10,
                TenantId = 1,
                Name = "Standard",
                TargetMinutes = 60,
                WarningMinutes = 30,
                IsActive = true
            },
            new()
            {
                Id = 11,
                TenantId = 2,
                Name = "Other",
                TargetMinutes = 60,
                WarningMinutes = 30,
                IsActive = true
            }
        };

        var caseRepositoryMock = new Mock<ICaseRepository>();
        var slaRepositoryMock = new Mock<ISlaRepository>();

        caseRepositoryMock
            .Setup(r => r.AsQueryable(It.IsAny<bool>()))
            .Returns(new TestAsyncEnumerable<Case>(cases.Where(c => !c.Erased).AsQueryable()));

        slaRepositoryMock
            .Setup(r => r.AsQueryable(It.IsAny<bool>()))
            .Returns(new TestAsyncEnumerable<Sla>(slas.Where(s => !s.Erased).AsQueryable()));

        var handler = new CheckSlaBatchQueryHandler(caseRepositoryMock.Object, slaRepositoryMock.Object);
        var request = new CheckSlaBatchRequest(TenantId: 1, Now: now, PageNumber: 1, PageSize: 10);

        var result = await handler.Handle(new CheckSlaBatchQuery(request), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        var item = result.Items.First();
        Assert.Equal(1, item.CaseId);
        Assert.True(item.IsBreached);
        Assert.True(item.IsActive);
    }

    private sealed class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner = inner;

        public IQueryable CreateQuery(Expression expression)
            => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression)
            => _inner.Execute(expression)!;

        public TResult Execute<TResult>(Expression expression)
            => _inner.Execute<TResult>(expression)!;

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            => Execute<TResult>(expression);

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            => new TestAsyncEnumerable<TResult>(expression);
    }

    private sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    private sealed class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner = inner;

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
            => new(_inner.MoveNext());
    }
}
