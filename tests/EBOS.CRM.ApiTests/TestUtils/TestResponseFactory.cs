using System.Reflection;

namespace EBOS.CRM.ApiTests.TestUtils;

public static class TestResponseFactory
{
    public static T Create<T>() where T : class
    {
        var type = typeof(T);
        var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor == null)
            throw new InvalidOperationException($"No public constructor found for {type.Name}.");

        var args = ctor.GetParameters()
            .Select(p => CreateDefaultValue(p.ParameterType))
            .ToArray();

        return (T)ctor.Invoke(args);
    }

    private static object? CreateDefaultValue(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
            return null;

        if (type == typeof(string))
            return string.Empty;
        if (type == typeof(bool))
            return true;
        if (type == typeof(int) || type == typeof(long) || type == typeof(short))
            return 1;
        if (type == typeof(decimal))
            return 0m;
        if (type == typeof(double))
            return 0d;
        if (type == typeof(float))
            return 0f;
        if (type == typeof(DateTime))
            return DateTime.UtcNow;

        return Activator.CreateInstance(type);
    }
}


