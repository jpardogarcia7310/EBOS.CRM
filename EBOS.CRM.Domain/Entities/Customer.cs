using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("Customers, CRM")]
public abstract class Customer : ErasableEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!; // CRM Internal Code
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = null!;
    [Required]
    [MaxLength(12)]
    public string Phone { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    // Foreign Keys
    public long? TaxInformationId { get; set; }
    public TaxInformation? TaxInformation { get; set; }

    public long? BankInformationId { get; set; }
    public BankInformation? BankInformation { get; set; }

    public CreditAccount? CreditAccount { get; set; }
    
    public long StatusId { get; set; }
    public Status Status { get; set; } = null!;

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
