using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("Empresa,CRM")]
public class Empresa : Cliente
{
    public string RazonSocial { get; set; } = null!;
    public string Cif { get; set; } = null!;

    public ICollection<Delegacion> Delegaciones { get; set; } = new List<Delegacion>();
}
