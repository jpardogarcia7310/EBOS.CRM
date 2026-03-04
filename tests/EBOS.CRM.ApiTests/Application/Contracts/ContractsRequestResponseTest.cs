using System.Reflection;
using EBOS.CRM.Contracts.Responses.Common;

namespace EBOS.CRM.ApiTests.Application.Contracts;

public class ContractsRequestResponseTest
{
    private static readonly DateTime FixedUtc =
        new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Requests_CanBeConstructed_WithSampleValues()
    {
        var requestTypes = GetContractTypes("EBOS.CRM.Contracts.Requests");

        Assert.NotEmpty(requestTypes);

        foreach (var type in requestTypes)
        {
            var instance = CreateInstance(type);
            Assert.NotNull(instance);

            AssertConstructorProperties(type, instance);
        }
    }

    [Fact]
    public void Responses_CanBeConstructed_WithSampleValues()
    {
        var responseTypes = GetContractTypes("EBOS.CRM.Contracts.Responses");

        Assert.NotEmpty(responseTypes);

        foreach (var type in responseTypes)
        {
            var instance = CreateInstance(type);
            Assert.NotNull(instance);

            AssertConstructorProperties(type, instance);
        }
    }

    private static IReadOnlyCollection<Type> GetContractTypes(string namespacePrefix)
    {
        var assembly = typeof(PagedResult<>).Assembly;

        var types = assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Namespace != null &&
                t.Namespace.StartsWith(namespacePrefix, StringComparison.Ordinal))
            .Select(EnsureClosedGeneric)
            .Where(t => t != null)
            .Select(t => t!)
            .ToList();

        return types;
    }

    private static Type? EnsureClosedGeneric(Type type)
    {
        if (!type.IsGenericTypeDefinition)
        {
            return type;
        }

        if (type == typeof(PagedResult<>))
        {
            return typeof(PagedResult<string>);
        }

        return null;
    }

    private static void AssertConstructorProperties(Type type, object instance)
    {
        var ctor = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor == null)
        {
            return;
        }

        var parameters = ctor.GetParameters();
        if (parameters.Length == 0)
        {
            return;
        }

        var args = CreateConstructorArgs(parameters);

        var rebuilt = ctor.Invoke(args);
        Assert.NotNull(rebuilt);

        foreach (var parameter in parameters)
        {
            var prop = type.GetProperty(parameter.Name!,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop == null)
            {
                continue;
            }

            var expected = args[parameter.Position];
            var actual = prop.GetValue(rebuilt);

            if (expected is null)
            {
                Assert.Null(actual);
                continue;
            }

            if (!prop.PropertyType.IsValueType &&
                prop.PropertyType != typeof(string))
            {
                Assert.Same(expected, actual);
                continue;
            }

            Assert.Equal(expected, actual);
        }
    }

    private static object?[] CreateConstructorArgs(ParameterInfo[] parameters)
    {
        var args = new object?[parameters.Length];
        for (var i = 0; i < parameters.Length; i++)
        {
            args[i] = CreateInstance(parameters[i].ParameterType, 0);
        }

        return args;
    }

    private static object? CreateInstance(Type type, int depth = 0)
    {
        if (depth > 3)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        if (type == typeof(string))
        {
            return "value";
        }

        if (type == typeof(int))
        {
            return 1;
        }

        if (type == typeof(long))
        {
            return 1L;
        }

        if (type == typeof(bool))
        {
            return true;
        }

        if (type == typeof(DateTime))
        {
            return FixedUtc;
        }

        if (type == typeof(DateTimeOffset))
        {
            return new DateTimeOffset(FixedUtc, TimeSpan.Zero);
        }

        if (type == typeof(TimeSpan))
        {
            return TimeSpan.FromMinutes(5);
        }

        if (type == typeof(Guid))
        {
            return Guid.Empty;
        }

        if (type == typeof(decimal))
        {
            return 1.23m;
        }

        var underlyingNullable = Nullable.GetUnderlyingType(type);
        if (underlyingNullable != null)
        {
            return CreateInstance(underlyingNullable, depth + 1);
        }

        if (type.IsEnum)
        {
            var values = Enum.GetValues(type);
            return values.Length > 0 ? values.GetValue(0) : Activator.CreateInstance(type);
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var elementValue = CreateInstance(elementType, depth + 1);
            var array = Array.CreateInstance(elementType, 1);
            array.SetValue(elementValue, 0);
            return array;
        }

        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();

            if (genericDef == typeof(IReadOnlyCollection<>) ||
                genericDef == typeof(ICollection<>) ||
                genericDef == typeof(IEnumerable<>))
            {
                var itemType = type.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(itemType);
                var list = (System.Collections.IList)Activator.CreateInstance(listType)!;
                list.Add(CreateInstance(itemType, depth + 1));
                return list;
            }
        }

        var ctor = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor == null)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        var args = ctor.GetParameters()
            .Select(p => CreateInstance(p.ParameterType, depth + 1))
            .ToArray();

        return ctor.Invoke(args);
    }
}
