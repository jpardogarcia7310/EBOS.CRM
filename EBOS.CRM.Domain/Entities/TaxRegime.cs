using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("TaxRegime, EBOS")]
public class TaxRegime : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = default!;

    public ICollection<Customer> Customers { get; set; } = [];
}