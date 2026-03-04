using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;

namespace EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;

public class CaseActivityRepository(CrmDbContext context) : BaseRepository<CaseActivity>(context),
    ICaseActivityRepository
{
    public async Task<bool> HasOpenByCaseIdAsync(long caseId, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(a => a.CaseId == caseId && a.Status != CaseActivity.StatusCompleted)
            .AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CaseActivity>> GetAllByCaseIdAsync(long caseId,
        CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(a => a.CaseId == caseId)
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CaseActivity>> GetAllByCaseIdPagedAsync(long caseId, int pageNumber,
        int pageSize, string? status = null, DateTime? from = null, DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = Math.Max(1, pageSize);

        var query = AsQueryable()
            .Where(a => a.CaseId == caseId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        if (from.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= to.Value);
        }

        return await query
            .OrderBy(a => a.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByCaseIdAsync(long caseId, CancellationToken cancellationToken = default)
    {
        return await AsQueryable()
            .Where(a => a.CaseId == caseId)
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountByCaseIdAsync(long caseId, string? status = null, DateTime? from = null,
        DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var query = AsQueryable()
            .Where(a => a.CaseId == caseId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status == status);
        }

        if (from.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= to.Value);
        }

        return await query.CountAsync(cancellationToken);
    }
}
