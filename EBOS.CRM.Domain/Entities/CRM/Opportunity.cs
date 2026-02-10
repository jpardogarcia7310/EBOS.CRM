using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Opportunity : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Name { get; set; } = null!;
    public long StageId { get; set; }
    public OpportunityStage Stage { get; set; } = null!;
    public long OwnerUserId { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime? ExpectedCloseDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Probability { get; set; }
    public string? Source { get; set; }
    public long? SourceLeadId { get; set; }
    public Lead? SourceLead { get; set; }
    public string? CloseReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
