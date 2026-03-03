using System.Security.Claims;
using EBOS.CRM.Contracts.Responses.CRM;

namespace EBOS.CRM.Api.Services;

public interface ICustomerPiiMaskingService
{
    CustomerResponse Mask(CustomerResponse response, bool applyMasking);
    CorporateCustomerResponse Mask(CorporateCustomerResponse response, bool applyMasking);
    IndividualCustomerResponse Mask(IndividualCustomerResponse response, bool applyMasking);
    IReadOnlyCollection<CustomerResponse> Mask(IReadOnlyCollection<CustomerResponse> responses, bool applyMasking);
    IReadOnlyCollection<CorporateCustomerResponse> Mask(IReadOnlyCollection<CorporateCustomerResponse> responses,
        bool applyMasking);
    IReadOnlyCollection<IndividualCustomerResponse> Mask(IReadOnlyCollection<IndividualCustomerResponse> responses,
        bool applyMasking);
}

public sealed class CustomerPiiMaskingService(IHttpContextAccessor accessor) : ICustomerPiiMaskingService
{
    private const string PiiReadPermission = "crm.customer.pii.read";

    public CustomerResponse Mask(CustomerResponse response, bool applyMasking)
    {
        if (!ShouldMask(applyMasking))
        {
            return response;
        }

        return response with
        {
            Email = MaskEmail(response.Email),
            Phone = MaskPhone(response.Phone)
        };
    }

    public CorporateCustomerResponse Mask(CorporateCustomerResponse response, bool applyMasking)
    {
        if (!ShouldMask(applyMasking))
        {
            return response;
        }

        return response with
        {
            Email = MaskEmail(response.Email),
            Phone = MaskPhone(response.Phone),
            TaxIdentification = MaskTaxId(response.TaxIdentification)
        };
    }

    public IndividualCustomerResponse Mask(IndividualCustomerResponse response, bool applyMasking)
    {
        if (!ShouldMask(applyMasking))
        {
            return response;
        }

        return response with
        {
            Email = MaskEmail(response.Email),
            Phone = MaskPhone(response.Phone),
            FirstName = MaskName(response.FirstName),
            LastName = MaskName(response.LastName),
            IdentificationNumber = MaskGeneric(response.IdentificationNumber)
        };
    }

    public IReadOnlyCollection<CustomerResponse> Mask(IReadOnlyCollection<CustomerResponse> responses, bool applyMasking)
        => responses.Select(x => Mask(x, applyMasking)).ToList();

    public IReadOnlyCollection<CorporateCustomerResponse> Mask(IReadOnlyCollection<CorporateCustomerResponse> responses,
        bool applyMasking)
        => responses.Select(x => Mask(x, applyMasking)).ToList();

    public IReadOnlyCollection<IndividualCustomerResponse> Mask(IReadOnlyCollection<IndividualCustomerResponse> responses,
        bool applyMasking)
        => responses.Select(x => Mask(x, applyMasking)).ToList();

    private bool ShouldMask(bool applyMasking)
    {
        if (!applyMasking)
        {
            return false;
        }

        return !CanReadPii();
    }

    private bool CanReadPii()
    {
        var user = accessor.HttpContext?.User;
        if (user is null || user.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        if (user.IsInRole("Admin") || user.IsInRole("PIIReader"))
        {
            return true;
        }

        var permissionClaims = user.FindAll("permissions").Select(x => x.Value)
            .Concat(user.FindAll("permission").Select(x => x.Value));
        return permissionClaims.Any(x => string.Equals(x, PiiReadPermission, StringComparison.OrdinalIgnoreCase));
    }

    private static string MaskEmail(string value)
    {
        var at = value.IndexOf('@');
        if (at <= 1)
        {
            return "***";
        }

        return $"{value[0]}***{value[(at - 1)..]}";
    }

    private static string MaskPhone(string value)
    {
        if (value.Length <= 4)
        {
            return new string('*', value.Length);
        }

        return new string('*', value.Length - 4) + value[^4..];
    }

    private static string MaskTaxId(string value)
    {
        return MaskKeepEnds(value, 2, 2);
    }

    private static string MaskName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value.Length == 1 ? "*" : value[0] + new string('*', Math.Max(1, value.Length - 1));
    }

    private static string? MaskGeneric(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return MaskKeepEnds(value, 1, 1);
    }

    private static string MaskKeepEnds(string value, int keepStart, int keepEnd)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (value.Length <= keepStart + keepEnd)
        {
            return new string('*', value.Length);
        }

        return value[..keepStart] + new string('*', value.Length - keepStart - keepEnd) + value[^keepEnd..];
    }
}
