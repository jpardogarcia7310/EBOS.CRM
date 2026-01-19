using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("DatosBancarios,CRM")]
public class DatosBancarios : ErasableEntity
{
    public string Iban { get; set; } = null!;
    public string? Bic { get; set; }
    public string? Banco { get; set; }

    public Cliente Cliente { get; set; } = null!;
}
