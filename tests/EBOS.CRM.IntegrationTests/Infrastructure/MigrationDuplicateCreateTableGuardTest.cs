using System.Text.RegularExpressions;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public sealed class MigrationDuplicateCreateTableGuardTest
{
    [Fact]
    public void Migrations_ShouldNotContain_DuplicateCreateTable_ForSameSchemaAndTable()
    {
        var migrationsPath = ResolveMigrationsPath();
        var migrationFiles = Directory
            .EnumerateFiles(migrationsPath, "*.cs", SearchOption.TopDirectoryOnly)
            .Where(path =>
                !path.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase) &&
                !path.EndsWith("CrmDbContextModelSnapshot.cs", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        migrationFiles.Should().NotBeEmpty("the migration guard requires migration source files");

        var createTableRegex = new Regex(
            @"CreateTable\(\s*name:\s*""(?<table>[^""]+)""(?<rest>[\s\S]*?)columns:\s*table",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var schemaRegex = new Regex(
            @"schema:\s*""(?<schema>[^""]+)""",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        var createdTables = new List<(string Key, string File)>();

        foreach (var file in migrationFiles)
        {
            var content = File.ReadAllText(file);
            var matches = createTableRegex.Matches(content);
            foreach (Match match in matches)
            {
                var table = match.Groups["table"].Value.Trim();
                var rest = match.Groups["rest"].Value;
                var schemaMatch = schemaRegex.Match(rest);
                var schema = schemaMatch.Success ? schemaMatch.Groups["schema"].Value.Trim() : "dbo";
                var key = $"{schema}.{table}".ToLowerInvariant();
                createdTables.Add((key, Path.GetFileName(file)));
            }
        }

        var duplicates = createdTables
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => $"{g.Key} => {string.Join(", ", g.Select(x => x.File))}")
            .ToArray();

        duplicates.Should().BeEmpty(
            "duplicate CreateTable found in migrations: {0}",
            string.Join(" | ", duplicates));
    }

    private static string ResolveMigrationsPath()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(
                current.FullName,
                "EBOS.CRM.Infrastructure",
                "Persistence",
                "Migrations");

            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate EBOS.CRM.Infrastructure/Persistence/Migrations from test base directory.");
    }
}
