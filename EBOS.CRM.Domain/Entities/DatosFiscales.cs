using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("DatosFiscales,CRM")]
public class DatosFiscales : ErasableEntity
{
    public string NombreFiscal { get; set; } = null!;
    public string Nif { get; set; } = null!;
    public string DireccionFiscal { get; set; } = null!;
    public string Ciudad { get; set; } = null!;
    public string CodigoPostal { get; set; } = null!;
    public string Pais { get; set; } = null!;

    public Cliente Cliente { get; set; } = null!;
}
