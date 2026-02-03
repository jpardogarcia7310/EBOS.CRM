using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using EBOS.CRM.Domain.Entities.CRM;


namespace EBOS.CRM.Domain.Entities;

public class Status : BaseEntity
{
    public string Description { get; set; } = null!;

    public ICollection<Customer> Customers { get; set; } = [];
}


