using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("TaxRegime, EBOS")]
public class TaxRegime : BaseEntity
{
    public string Description { get; set; } = default!;

    public ICollection<Customer> Customers { get; set; } = [];
}