using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("Estados, CRM")]
public class Estado : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = default!;

    public ICollection<Cliente> Clientes { get; set; } = [];
}
