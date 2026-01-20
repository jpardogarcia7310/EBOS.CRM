using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public class BankInformation : ErasableEntity
{
    [Required]
    [MaxLength(34)]
    public string Iban { get; set; } = null!;
    [MaxLength(11)]
    public string? Bic { get; set; }
    [MaxLength(200)]
    public string? BankName { get; set; }

    // Foreign Keys
    public Customer Customer { get; set; } = null!;
}
