using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("Statuses, CRM")]
public class Status : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = null!;

    public ICollection<Customer> Customers { get; set; } = [];
}
