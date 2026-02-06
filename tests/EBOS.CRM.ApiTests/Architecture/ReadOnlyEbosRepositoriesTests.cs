using EBOS.CRM.Domain.Interfaces.Repositories;
using FluentAssertions;
using System.Reflection;
using EBOS.Core.Primitives.Interfaces;

namespace EBOS.CRM.ApiTests.Architecture;

public class ReadOnlyEbosRepositoriesTests
{
    private static readonly Type[] EbosRepositoryInterfaces =
    [
        typeof(IAddressTypeRepository),
        typeof(ICountryRepository),
        typeof(IIdentificationTypeRepository),
        typeof(IStatusRepository)
    ];

    [Fact]
    public void EbosRepositories_ShouldNotExposeCommands()
    {
        var forbidden = new HashSet<string>(StringComparer.Ordinal)
        {
            nameof(IRepository<object>.AddAsync),
            nameof(IRepository<object>.AddRangeAsync),
            nameof(IRepository<object>.AttachAsync),
            nameof(IRepository<object>.UpdateAsync),
            nameof(IRepository<object>.DeleteAsync)
        };

        foreach (var repoType in EbosRepositoryInterfaces)
        {
            var methodNames = repoType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(m => m.Name)
                .ToHashSet(StringComparer.Ordinal);

            var conflicts = methodNames.Intersect(forbidden).ToArray();
            conflicts.Should().BeEmpty($"{repoType.Name} debe ser solo lectura");
        }
    }

    [Fact]
    public void EbosConcreteRepositories_ShouldNotExposeCommands()
    {
        var forbidden = new HashSet<string>(StringComparer.Ordinal)
        {
            nameof(IRepository<object>.AddAsync),
            nameof(IRepository<object>.AddRangeAsync),
            nameof(IRepository<object>.AttachAsync),
            nameof(IRepository<object>.UpdateAsync),
            nameof(IRepository<object>.DeleteAsync)
        };

        var concreteRepositories = new[]
        {
            typeof(EBOS.CRM.Infrastructure.Repositories.Concrete.AddressTypeRepository),
            typeof(EBOS.CRM.Infrastructure.Repositories.Concrete.CountryRepository),
            typeof(EBOS.CRM.Infrastructure.Repositories.Concrete.IdentificationTypeRepository),
            typeof(EBOS.CRM.Infrastructure.Repositories.Concrete.StatusRepository)
        };

        foreach (var repoType in concreteRepositories)
        {
            var methodNames = repoType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(m => m.Name)
                .ToHashSet(StringComparer.Ordinal);

            var conflicts = methodNames.Intersect(forbidden).ToArray();
            conflicts.Should().BeEmpty($"{repoType.Name} debe ser solo lectura");
        }
    }
}
