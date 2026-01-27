using System.ComponentModel.DataAnnotations;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CorporateCustomer : Customer
{
    [Required]
    [MaxLength(200)]
    public string LegalName { get; set; } = null!;
    [Required]
    [MaxLength(20)]
    public string TaxIdentification { get; set; } = null!;

    public ICollection<BranchOffice> BranchOffices { get; set; } = new List<BranchOffice>();
}
