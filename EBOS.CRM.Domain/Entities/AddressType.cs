using System.ComponentModel.DataAnnotations;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

public class AddressType : ErasableEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!; 
    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = null!; 
    
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
