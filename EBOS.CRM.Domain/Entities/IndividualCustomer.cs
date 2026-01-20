using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

public class IndividualCustomer : Customer
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
    public string? IdentityDocument { get; set; } // DNI/NIE
}
