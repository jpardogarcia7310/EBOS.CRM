using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public sealed class IndividualCustomer : Customer
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string? IdentificationNumber { get; set; } // DNI/NIE

    public long IdentificationTypeId { get; set; }
    public IdentificationType IdentificationType { get; set; } = null!;

    public ICollection<AccountContact> AccountContacts { get; set; } = new List<AccountContact>();
}

