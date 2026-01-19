using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("Delegacion,CRM")]
public class Delegacion : ErasableEntity
{
    public long EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;

    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string Ciudad { get; set; } = null!;
    public string CodigoPostal { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public string? Telefono { get; set; }
}
