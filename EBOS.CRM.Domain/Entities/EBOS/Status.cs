using System;
using System.Collections.Generic;
using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Domain.Entities;

public class Status : BaseEntity
{
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<Customer> Customers { get; set; } = [];
}


