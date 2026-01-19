using System.ComponentModel.DataAnnotations.Schema;
using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities;

[Table("Direccion,CRM")]
public class Direccion : ErasableEntity
{
    public long ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public string Tipo { get; set; } = "Principal"; // Fiscal, Comercial, Envío, etc.

    public string Calle { get; set; } = null!;
    public string NumeroExterno { get; set; } = null!;
    public string? NumeroInterno { get; set; }

    public string? EntreCalle1 { get; set; }
    public string? EntreCalle2 { get; set; }

    public string? Barrio { get; set; }
    public string Localidad { get; set; } = null!;
    public string Provincia { get; set; } = null!;
    public long PaisId { get; set; } 
    public Pais Pais { get; set; }
    public string CodigoPostal { get; set; } = null!;

    public string? UrlGoogleMaps { get; set; }

    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
}
