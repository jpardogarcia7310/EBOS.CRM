using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

public sealed class IndividualCustomer : Customer
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;
    [Required]
    public DateTime? BirthDate { get; set; }
    [Required]
    [MaxLength(10)]
    public string? IdentificationNumber { get; set; } // DNI/NIE
    
    public long IdentificationTypeId { get; set; }
    [ForeignKey(nameof(IdentificationTypeId))]
    public IdentificationType IdentificationType { get; set; } = null!;
}
