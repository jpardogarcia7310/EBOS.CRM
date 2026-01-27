using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using AddressEntity = EBOS.CRM.Domain.Entities.CRM.Address;
using MapsterMapper;
using MediatR;
using System.Globalization;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;

public class AddAddressCommandHandler(IAddressRepository repository, IMapper mapper) :
    IRequestHandler<AddAddressCommand, AddressResponse>
{
    private readonly IAddressRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<AddressResponse> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var addressRequest = request.AddressRequest ??
                             throw new ArgumentNullException(nameof(request.AddressRequest));

        if (addressRequest.IsPrimary &&
            await _repository.ExistPrimaryAddressInCustomerId(addressRequest.CustomerId, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Customer {addressRequest.CustomerId} already has a primary address.");
        }

        var entity = new AddressEntity()
        {
            IsPrimary = addressRequest.IsPrimary,
            Street = addressRequest.Street,
            ExternalNumber = addressRequest.ExternalNumber,
            InternalNumber = addressRequest.InternalNumber,
            BetweenStreet1 = addressRequest.BetweenStreet1,
            BetweenStreet2 = addressRequest.BetweenStreet2,
            Neighbourhood = addressRequest.Neighbourhood,
            City = addressRequest.City,
            StateOrProvince = addressRequest.StateOrProvince,
            PostalCode = addressRequest.PostalCode,
            GoogleMapsUrl = addressRequest.GoogleMapsUrl,
            Latitude = ParseNullableDouble(addressRequest.Latitude),
            Longitude = ParseNullableDouble(addressRequest.Longitude),
            CustomerId = addressRequest.CustomerId,
            CountryId = addressRequest.CountryId,
            AddressTypeId = addressRequest.AddressTypeId
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AddressResponse>(entity);
    }

    private static double? ParseNullableDouble(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture,
            out parsed) ? parsed : null;
    }
}
