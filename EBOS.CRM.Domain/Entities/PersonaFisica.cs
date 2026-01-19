using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.CRM.Domain.Entities;

[Table("PersonaFisica,CRM")]
public class PersonaFisica : Cliente
{
    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Apellidos { get; set; } = null!;
    [Required]
    public DateTime? FechaNacimiento { get; set; }
    [Required]
    [MaxLength(10)]
    public string? DocumentoIdentidad { get; set; } // DNI/NIE
}
